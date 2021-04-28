using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _movementSpeed = 10f;
    [SerializeField]
    private float _rotationSpeed = 200f;
    [SerializeField]
    private float _yDestroyDistance = 12f;
    private Rigidbody2D _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        AssignTarget();
    }

    void Update()
    {
        if (transform.position.y > _yDestroyDistance)
        {
            if (this.gameObject.transform.parent != null)
            {
                Destroy(this.gameObject.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (_target != null)
        {
            Vector2 direction = (Vector2)_target.position - _rigidBody.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            _rigidBody.angularVelocity = -_rotationSpeed * rotateAmount;
            _rigidBody.velocity = transform.up * _movementSpeed;
        }
        else
        {
            AssignTarget();
            _rigidBody.velocity = transform.up * _movementSpeed;
        }        
    }

    public void AssignTarget()
    {
        Transform enemy = null;
        float distance;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemy == null)
            {
                enemy = enemies[i].transform;
                continue;
            }

            distance = Vector3.Distance(enemy.position, transform.position);
            float nextDistance = Vector3.Distance(enemies[i].transform.position, transform.position);

            if (nextDistance < distance)
            {
                distance = nextDistance;
                enemy = enemies[i].transform;
            }
        }

        _target = enemy;
    }
}
