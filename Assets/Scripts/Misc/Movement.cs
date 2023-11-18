using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;
    [SerializeField] private float _moveSpeed = 10f;

    private float _moveX;
    private bool _canMove = true;

    private Rigidbody2D _rigidBody;
    private Knockback _knockback;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnEnable()
    {
        _knockback.OnKnockbackStart += CanMoveFalse;
        _knockback.OnKnockbackEnd += CanMoveTrue;
    }
    private void OnDisable()
    {
        _knockback.OnKnockbackStart -= CanMoveFalse;
        _knockback.OnKnockbackEnd -= CanMoveTrue;
    }
    public void SetCurrentDirection(float currentDirection)
    {
        _moveX = currentDirection;
    }

    private void CanMoveTrue()
    {
        _canMove = true;
    }

    private void CanMoveFalse()
    {
        _canMove = false;
    }
    private void Move()
    {
        if (!_canMove) { return; }
        {
            Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.velocity.y);
            _rigidBody.velocity = movement;
        }
    }
}
