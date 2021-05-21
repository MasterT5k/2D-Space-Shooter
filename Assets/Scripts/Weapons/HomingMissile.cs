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
    private float _fuseTimer = 5f;
    private float _timer;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    private Rigidbody2D _rigidBody;
    private bool _isEnemyMissile = false;
    private bool _isReused = false;
    private PoolManager _poolManager = null;

    void OnDisable()
    {
        if (_isReused == true)
        {
            transform.position = transform.parent.position;
            _isEnemyMissile = false;
            _timer = 0;
            _target = null;
            _rigidBody.angularVelocity = 0;
            transform.rotation = Quaternion.Euler(Vector3.zero); 
        }
    }

    void Start()
    {
        _poolManager = GameObject.Find("Pool Manager").GetComponent<PoolManager>();
        if (_poolManager == null)
        {
            Debug.LogError("Pool Manager is NULL");
        }

        _rigidBody = GetComponent<Rigidbody2D>();
        if (_isEnemyMissile == false)
        {
            AssignEnemyTarget();
        }
        _isReused = true;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _fuseTimer)
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
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
        }
        else if (_target == null && _isEnemyMissile == false)
        {
            AssignEnemyTarget();
        }
        else
        {
            _rigidBody.angularVelocity = 0;
        }
        _rigidBody.velocity = transform.up * _movementSpeed;
    }

    public void AssignEnemyTarget()
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

    public void AssignEnemyMissile(Transform player)
    {
        _isEnemyMissile = true;
        if (player != null)
        {
            _target = player;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyMissile == true)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ChangeLives();
            }
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
