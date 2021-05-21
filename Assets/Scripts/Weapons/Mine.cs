using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private Explosion _explosionPrefab = null;

    private bool _isStopped = false;
    private bool _isReused = false;
    private Rigidbody2D _rigidbody = null;
    private Transform _target = null;
    private PoolManager _poolManager = null;

    private void OnDisable()
    {
        if (_isReused == true)
        {
            transform.position = transform.parent.position;
        }
    }

    void Start()
    {
        _poolManager = GameObject.Find("Pool Manager").GetComponent<PoolManager>();
        if (_poolManager == null)
        {
            Debug.LogError("Pool Manager is NULL");
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        _isReused = true;
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
            }
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }

        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }

        if (other.tag == "Omni Shot")
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
