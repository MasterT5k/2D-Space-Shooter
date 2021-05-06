using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveEnemy : Enemy
{
    [Header("Aggressive Settings")]
    [SerializeField]
    private float _ramRange = 5f;
    [SerializeField]
    private float _rammingSpeed = 6f;
    [SerializeField]
    private float _recoveryTime = 5f;
    private float _hitPlayerOffset = 0.8f;
    private bool _isInRamRange = false;
    private bool _isRecovering = false;

    protected override void Start()
    {
        base.Start();
        CheckPlayerRange();
    }

    protected override void Update()
    {
        if (_isRecovering == false)
        {
            CheckPlayerRange();
        }

        if (_isInRamRange == false)
        {
            base.Update();
        }
        else if (_isInRamRange == true && _isDead == false)
        {
            RamPlayer();
        }
    }

    void RamPlayer()
    {
        Vector2 playerPosition = _player.transform.position;
        float adjustedSpeed = _rammingSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, adjustedSpeed);
    }

    private void CheckPlayerRange()
    {
        float distance = GetDistanceFromPlayer();
        if (distance < _hitPlayerOffset)
        {
            _isInRamRange = false;

            if (_isDead == true)
            {
                return;
            }

            StartCoroutine(RecoverRoutine());
            return;
        }

        if (distance <= _ramRange)
        {
            _isInRamRange = true;
        }
        else
        {
            _isInRamRange = false;
        }
    }

    private float GetDistanceFromPlayer()
    {
        if (_player != null)
        {
            float playerYAxis = _player.transform.position.y;
            float enemyYAxis = transform.position.y;
            if (playerYAxis > enemyYAxis)
            {
                return Mathf.Infinity;
            }

            Vector2 currentPosition = transform.position;
            Vector2 playerPosition = _player.transform.position;

            float distance = Vector2.Distance(currentPosition, playerPosition);
            return distance;
        }
        return Mathf.Infinity;
    }

    IEnumerator RecoverRoutine()
    {
        _isRecovering = true;
        yield return new WaitForSeconds(_recoveryTime);
        _isRecovering = false;
    }

    //Magnetic Mine Enemy
    //void FixedUpdate()
    //{
    //    if (_isInRamRange == true && _isDead == false)
    //    {
    //        //_isOutOfStartRotation = true;
    //        Vector2 direction = (Vector2)_player.transform.position - _rigidBody.position;
    //        direction.Normalize();
    //        float rotateAmount = Vector3.Cross(direction, -transform.up).z;
    //        _rigidBody.angularVelocity = -_rotationSpeed * rotateAmount;
    //        _rigidBody.velocity = -transform.up * _rammingSpeed;
    //    }
    //    else if (_isInRamRange == false && _isDead == false)
    //    {
    //        _rigidBody.velocity = Vector2.zero;
    //        _rigidBody.angularVelocity = 0;
    //    }
    //}
}
