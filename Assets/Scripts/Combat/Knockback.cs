using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd;

    [SerializeField] private float _knockbackTime = .2f;

    private Vector3 _hitDirection;
    private float _knockbackThrust;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        OnKnockbackStart += ApplyKnockBackForce;
        OnKnockbackEnd += StopKnockRoutine;
    }
    private void OnDisable()
    {
        OnKnockbackStart -= ApplyKnockBackForce;
        OnKnockbackEnd -= StopKnockRoutine;
    }
    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust)
    {
        _hitDirection = hitDirection;
        _knockbackThrust = knockbackThrust;

        OnKnockbackStart?.Invoke();
    }

    private void ApplyKnockBackForce()
    {
        Vector3 difference = (transform.position - _hitDirection).normalized * _knockbackThrust * _rigidBody.mass;
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockbackTime);
        OnKnockbackEnd?.Invoke();
    }

    private void StopKnockRoutine()
    {
        _rigidBody.velocity = Vector2.zero;
    }
}
