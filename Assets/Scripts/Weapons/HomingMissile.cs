using System;
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
    private int _damage = 1;
    [SerializeField]
    private float _fuseTimer = 5f;
    private float _timer;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    private Rigidbody2D _rigidBody;
    private bool _isEnemyMissile = false;
    private bool _isReused = false;

    public static event Action<int> OnDamagePlayer;
    public static event Func<List<GameObject>> OnGetTargetList;
    public static event Func<int, GameObject> OnGetExplosion;

    private void OnEnable()
    {
        Enemy.OnDestroyed += RemoveTarget;
        Player.OnRemoveTarget += RemoveTarget;
    }

    void OnDisable()
    {
        Enemy.OnDestroyed -= RemoveTarget;
        Player.OnRemoveTarget -= RemoveTarget;

        if (_isReused == true)
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion(explosionID);
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.SetActive(true);
            }
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
            _rigidBody.angularVelocity = 0;
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
        float distance = Mathf.Infinity;
        List<GameObject> enemyList = OnGetTargetList?.Invoke();
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemy == null)
            {
                enemy = enemyList[i].transform;
                distance = Vector3.Distance(enemy.position, transform.position);
                continue;
            }
            
            float nextDistance = Vector3.Distance(enemyList[i].transform.position, transform.position);

            if (nextDistance < distance)
            {
                distance = nextDistance;
                enemy = enemyList[i].transform;
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
            OnDamagePlayer?.Invoke(-_damage);
            gameObject.SetActive(false);
        }
    }

    private void RemoveTarget(GameObject target)
    {
        if (_target == target.transform)
        {
            _target = null;
        }
    }
}
