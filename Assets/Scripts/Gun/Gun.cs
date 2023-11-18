using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;

public class Gun : MonoBehaviour
{
    // Action to trigger when the gun shoots.
    public static Action OnShoot;
    public static Action OnGrenadeShoot;

    [SerializeField] private Transform _bulletSpawnPoint;
    [Header("Bullet")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = .5f;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _muzzleFlashTime = .05f;
    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _grenadeShootCD = .8f;

    // Variables to store the mouse position and the last fire time.
    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool;
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private Vector2 _mousePos;
    private float _lastFireTime = 0f;
    private float _lastGrenadeTime = 0f;

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private CinemachineImpulseSource _impulseSource;
    private Animator _animator;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInput;
    }

    private void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        GatherInput();
        Shoot(); // Check for shooting input.
        RotateGun(); // Adjust the gun's rotation based on the mouse position.
    }

    private void OnEnable()
    {
        // Subscribe to the shoot action with two methods.
        OnShoot += ShootProjectile; 
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
        OnShoot += MuzzleFlash;
        OnGrenadeShoot += ShootGrenade;
        OnGrenadeShoot += FireAnimation;
        OnGrenadeShoot += ResetLastGrenadeShootTime;
    }

    private void OnDisable()
    {     
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzleFlash;
        OnGrenadeShoot -= ShootGrenade;
        OnGrenadeShoot -= FireAnimation;
        OnGrenadeShoot -= ResetLastGrenadeShootTime;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() => {
            return Instantiate(_bulletPrefab);
        }, bullet => {
            bullet.gameObject.SetActive(true);
        }, bullet => {
            bullet.gameObject.SetActive(false);
        }, bullet => {
            Destroy(bullet);
        }, false, 20, 40);
    }


    private Bullet CreateNewBullet()
    {
        return Instantiate(_bulletPrefab);
    }

    private void ActiveBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    // This method checks for shooting input and triggers the shoot action if conditions are met.
    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime)
        {
            OnShoot?.Invoke(); // Trigger the shoot action.
        }

        if (_frameInput.Grenade && Time.time >= _lastGrenadeTime)
        {
            OnGrenadeShoot?.Invoke();
        }
    }

    // This method instantiates a bullet and initializes it with a position and direction.
    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();

        // Check if the bullet is null before initializing it
        if (newBullet != null)
        {
            newBullet.Init(this, _bulletSpawnPoint.position, _mousePos);
        }
    }

    private void ShootGrenade()
    {
        Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastGrenadeTime = Time.time;
    }

    //This method handles the animation of the gun firing.
    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }
    // This method updates the last fire time to enforce the fire cooldown.
    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }
    private void ResetLastGrenadeShootTime()
    {
        _lastGrenadeTime = Time.time + _grenadeShootCD;
    }
    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    // This method adjusts the rotation of the gun based on the mouse position.
    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void MuzzleFlash()
    {
        if (_muzzleFlashRoutine != null)
        {
            StopCoroutine(_muzzleFlashRoutine);
        }

        _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }
}
