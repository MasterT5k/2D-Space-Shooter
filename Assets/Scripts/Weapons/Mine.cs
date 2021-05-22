using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private int _damage = 1;
    [SerializeField]
    private int _scoreValue = 5;
    [SerializeField]
    private Explosion _explosionPrefab = null;

    private bool _isStopped = false;
    private bool _isReused = false;
    private Rigidbody2D _rigidbody = null;
    private Transform _target = null;

    public static event Action<int> OnScored;
    public static event Action<int> OnDamagePlayer;
    public static event Func<int, GameObject> OnGetExplosion;

    private void OnDisable()
    {
        if (_isReused == true)
        {
            transform.position = transform.parent.position;
        }
    }

    void Start()
    {
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
            OnDamagePlayer?.Invoke(-_damage);
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.SetActive(true); 
            }
            gameObject.SetActive(false);
        }

        if (other.tag == "Laser")
        {
            OnScored?.Invoke(_scoreValue);
            other.gameObject.SetActive(false);
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }

        if (other.tag == "Omni Shot")
        {
            OnScored?.Invoke(_scoreValue);
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
