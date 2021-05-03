using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagEnemy : Enemy
{
    [Header("ZigZag Variables")]
    [SerializeField]
    private float _sideMovementSpeed = 2f;
    [SerializeField]
    private float _xOffset = 1.5f;
    private bool _isMovingLeft = false;
    private float _xAxis;

    protected override void Start()
    {
        base.Start();

        _xAxis = transform.position.x;

        int randomChance = Random.Range(0, 2);
        if (randomChance == 0)
        {
            _isMovingLeft = true;
        }        
    }

    protected override void CalulateMovement()
    {
        base.CalulateMovement();

        if (_isDead == false)
        {
            ZigZagMovement();
        }
    }

    protected override void Respawn()
    {
        base.Respawn();

        _xAxis = transform.position.x;
    }

    void ZigZagMovement()
    {
        float adjustedSpeed = _sideMovementSpeed * Time.deltaTime;
        if (_isMovingLeft == false)
        {
            Vector2 rightPosition = new Vector2(_xAxis + _xOffset, transform.position.y);
            transform.position = Vector3.MoveTowards(transform.position, rightPosition, adjustedSpeed);
            if (transform.position.x == rightPosition.x)
            {
                _isMovingLeft = true;
            }
        }
        else
        {
            Vector2 leftPosition = new Vector2(_xAxis - _xOffset, transform.position.y);            
            transform.position = Vector3.MoveTowards(transform.position, leftPosition, adjustedSpeed);
            if (transform.position.x == leftPosition.x)
            {
                _isMovingLeft = false;
            }
        }
    }
}
