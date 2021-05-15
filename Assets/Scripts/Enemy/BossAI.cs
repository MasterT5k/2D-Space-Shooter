using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _rotSpeed = 10f;

    [SerializeField]
    private Vector2 _finalPosition = Vector2.zero;
    private bool _inPosition = false;

    [Header("Weapon Settings")]
    [SerializeField]
    private float _stageDuration = 5f;
    [SerializeField]
    private float _fireRate = 2f;
    private float _canFire;

    [SerializeField]
    private GameObject _missilePrefab = null;
    [SerializeField]
    private Transform[] _missleSpawnPoints = null;

    [SerializeField]
    private List<BeamEmitter> _destroyObj = new List<BeamEmitter>();
    private bool _beamsActive = false;

    [Header("Health Settings")]
    [SerializeField]
    private int _reactorHealth = 4;
    private int _currentReactorHealth;
    [SerializeField]
    private GameObject _reactorCoreObj = null;
    [SerializeField]
    private GameObject _explosionPrefab = null;

    private bool _inFinalStage = false;

    private Player _player = null;

    private EnemyState _currentState = EnemyState.Idle;
    private enum EnemyState
    {
        FiringBeams,
        FiringMissiles,
        Idle
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is Null.");
        }

        if (_destroyObj.Count > 0)
        {
            _destroyObj.RemoveAll(empty => empty == null);
            for (int i = 0; i < _destroyObj.Count; i++)
            {
                _destroyObj[i].gameObject.SetActive(true);
            }
        }

        _currentReactorHealth = _reactorHealth;
        _reactorCoreObj.SetActive(false);
    }

    void Update()
    {
        if (_inPosition == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, _finalPosition, _speed * Time.deltaTime);
            if (transform.position == (Vector3)_finalPosition)
            {
                _inPosition = true;
                StartCoroutine(BossFightCoroutine());
            }
        }
        else
        {
            transform.Rotate(Vector3.forward * _rotSpeed * Time.deltaTime);
        }

        if (_currentState == EnemyState.FiringBeams && _beamsActive == false)
        {
            _beamsActive = true;
            LaserBeams(_beamsActive);

        }
        else if (_currentState == EnemyState.FiringMissiles && Time.time >= _canFire)
        {
            if (_beamsActive == true)
            {
                _beamsActive = false;
                LaserBeams(_beamsActive);
            }
            FireMissiles();
        }
    }

    void LaserBeams(bool turnOn)
    {
        _destroyObj.RemoveAll(empty => empty == null);
        if (_destroyObj.Count <= 0)
        {
            Debug.Log("All Emmiters Destroyed!");
            _inFinalStage = true;
            return;
        }

        if (turnOn == true)
        {
            for (int i = 0; i < _destroyObj.Count; i++)
            {
                GameObject obj = _destroyObj[i].gameObject;
                if (obj.activeInHierarchy == true)
                {
                    _destroyObj[i].LaserBeams(turnOn);
                }
            }
        }
        else
        {
            for (int i = 0; i < _destroyObj.Count; i++)
            {
                GameObject obj = _destroyObj[i].gameObject;
                if (obj.activeInHierarchy == true)
                {
                    _destroyObj[i].LaserBeams(turnOn);
                }
            }
        }
    }

    void FireMissiles()
    {
        _canFire = _fireRate + Time.time;

        if (_missleSpawnPoints.Length >= 2)
        {
            Vector3 spawnPoint;
            GameObject missile;
            for (int i = 0; i < _missleSpawnPoints.Length; i++)
            {
                spawnPoint = _missleSpawnPoints[i].position;
                missile = Instantiate(_missilePrefab, spawnPoint, transform.rotation);
                if (_player != null)
                {
                    missile.GetComponent<HomingMissile>().AssignEnemyMissile(_player.transform); 
                }
            } 
        }
        else if (_missleSpawnPoints.Length > 0)
        {
            GameObject missile;
            Vector3 spawnPoint = _missleSpawnPoints[0].position;
            missile = Instantiate(_missilePrefab, spawnPoint, Quaternion.identity);
            if (_player != null)
            {
                missile.GetComponent<HomingMissile>().AssignEnemyMissile(_player.transform);
            }
        }
    }

    IEnumerator BossFightCoroutine()
    {
        while (_player != null && _inFinalStage == false)
        {
            _currentState = EnemyState.FiringMissiles;
            yield return new WaitForSeconds(_stageDuration);

            _currentState = EnemyState.FiringBeams;
            yield return new WaitForSeconds(_stageDuration);
        }

        if (_inFinalStage == false)
        {
            _currentState = EnemyState.Idle; 
        }
        else
        {
            FinalStage();
        }
    }

    void FinalStage()
    {
        Debug.Log("Entered Final Stage!");
        _currentState = EnemyState.FiringMissiles;
        _reactorCoreObj.SetActive(true);
    }

    void DamageCore(int amount = -1)
    {
        _currentReactorHealth += amount;
        if (_currentReactorHealth <= 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            DamageCore();
        }

        if (other.tag == "Omni Shot")
        {
            DamageCore();
        }
    }
}
