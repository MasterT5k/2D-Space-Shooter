using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private GameObject _explosionPrefab = null;

    private bool _isStopped = false;
    private Rigidbody2D _rigidbody = null;
    private Transform _target = null;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (_target != null)
        {
            if (_isStopped == true)
            {
                _isStopped = false;
            }

            Vector2 direction = (Vector2)_target.position - _rigidbody.position;
            direction.Normalize();
            _rigidbody.velocity = direction * _speed;
        }
        else if (_target == null && _isStopped == false)
        {            
            _rigidbody.velocity = Vector2.MoveTowards(_rigidbody.velocity, Vector2.zero, _speed * Time.deltaTime);

            if (_rigidbody.velocity.x == 0 && _rigidbody.velocity.y == 0)
            {
                _isStopped = true;
            }
        }
    }

    public void RecieveTarget(Transform target)
    {
        if (_target == null)
        {
            _target = target;
        }
    }

    public void LoseTarget(Transform target)
    {
        if (_target == target)
        {
            _target = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ChangeLives();
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (other.tag == "Omni Shot")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
