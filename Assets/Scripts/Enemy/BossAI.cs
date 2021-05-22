using System;
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
    private AudioClip _laserBeamClip = null;

    [SerializeField]
    private WeaponID _missilePrefab = null;
    [SerializeField]
    private Transform[] _missileSpawnPoints = null;

    [SerializeField]
    private List<BeamEmitter> _beamEmitterList = new List<BeamEmitter>();
    private bool _beamsActive = false;
    [SerializeField]
    private int _activeEmitters;

    [Header("Health Settings")]
    [SerializeField]
    private int _reactorHealth = 4;
    private int _currentReactorHealth;
    [SerializeField]
    private int _scoreValue = 100;
    [SerializeField]
    private GameObject _reactorCoreObj = null;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    [SerializeField]
    private GameObject _coverShield = null;

    private bool _inFinalStage = false;

    private Player _player = null;

    public static event Action<GameObject> OnActiveReactor;
    public static event Action OnDestroyed;
    public static event Action<int> OnScored;
    public static event Action<AudioClip> OnPlayLoopingSFX;
    public static event Action OnStopLoopingSFX;
    public static event Func<int, GameObject> OnGetWeapon;
    public static event Func<int, GameObject> OnGetExplosion;
    public static event Func<GameObject> OnGetPlayerObject;
    public static event Func<Player> OnGetPlayerScript;

    private EnemyState _currentState = EnemyState.Idle;
    private enum EnemyState
    {
        FiringBeams,
        FiringMissiles,
        Idle
    }

    private void OnEnable()
    {
        _coverShield.SetActive(true);
        _activeEmitters = 0;
        if (_beamEmitterList.Count > 0)
        {
            for (int i = 0; i < _beamEmitterList.Count; i++)
            {
                _activeEmitters++;
                _beamEmitterList[i].gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        GameObject playerObj = OnGetPlayerObject?.Invoke();
        
        if (playerObj != null)
        {
            _player = OnGetPlayerScript?.Invoke();
        }
        
        if (playerObj.activeInHierarchy == false)
        {
            Debug.Log("Player is dead!");
            gameObject.SetActive(false);
        }

        _currentReactorHealth = _reactorHealth;
        _reactorCoreObj.SetActive(false);
    }

    void Update()
    {
        if (_activeEmitters <= 0)
        {
            OnStopLoopingSFX?.Invoke();
            _inFinalStage = true;
        }
        else
        {
            CheckEmitters();
        }

        if (_inPosition == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, _finalPosition, _speed * Time.deltaTime);
            if (transform.position == (Vector3)_finalPosition)
            {
                for (int i = 0; i < _beamEmitterList.Count; i++)
                {
                    _beamEmitterList[i].ActivateCollider();
                }
                _inPosition = true;
                _coverShield.SetActive(false);
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

    private void CheckEmitters()
    {
        int activeEmitters = 0;
        for (int i = 0; i < _beamEmitterList.Count; i++)
        {
            bool active = _beamEmitterList[i].gameObject.activeInHierarchy;
            if (active == true)
            {
                activeEmitters++;
            }
        }

        _activeEmitters = activeEmitters;
    }

    void LaserBeams(bool turnOn)
    {

        if (_activeEmitters <= 0)
        {
            OnStopLoopingSFX?.Invoke();
            _inFinalStage = true;
            return;
        }

        if (turnOn == true)
        {
            OnPlayLoopingSFX?.Invoke(_laserBeamClip);
            for (int i = 0; i < _beamEmitterList.Count; i++)
            {
                GameObject obj = _beamEmitterList[i].gameObject;
                if (obj.activeInHierarchy == true)
                {
                    _beamEmitterList[i].LaserBeams(turnOn);
                }
            }
        }
        else
        {
            OnStopLoopingSFX?.Invoke();
            for (int i = 0; i < _beamEmitterList.Count; i++)
            {
                GameObject obj = _beamEmitterList[i].gameObject;
                if (obj.activeInHierarchy == true)
                {
                    _beamEmitterList[i].LaserBeams(turnOn);
                }
            }
        }
    }

    void FireMissiles()
    {
        _canFire = _fireRate + Time.time;

        if (_missileSpawnPoints.Length >= 2)
        {
            GameObject missile;
            for (int i = 0; i < _missileSpawnPoints.Length; i++)
            {
                int weaponType = _missilePrefab.GetWeaponType();
                missile = OnGetWeapon?.Invoke(weaponType);
                if (missile != null)
                {
                    missile.transform.position = _missileSpawnPoints[i].position;
                    missile.SetActive(true);
                    if (_player != null)
                    {
                        missile.GetComponent<HomingMissile>().AssignEnemyMissile(_player.transform);
                    } 
                }
            } 
        }
        else if (_missileSpawnPoints.Length > 0)
        {
            GameObject missile;
            int weaponType = _missilePrefab.GetWeaponType();
            missile = OnGetWeapon?.Invoke(weaponType);
            if (missile != null)
            {
                missile.transform.position = _missileSpawnPoints[0].position;
                missile.SetActive(true);
                if (_player != null)
                {
                    missile.GetComponent<HomingMissile>().AssignEnemyMissile(_player.transform);
                }
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
        _currentState = EnemyState.FiringMissiles;
        OnActiveReactor?.Invoke(_reactorCoreObj);
        _reactorCoreObj.SetActive(true);
    }

    void DamageCore(int amount = -1)
    {
        _currentReactorHealth += amount;
        if (_currentReactorHealth <= 0)
        {
            OnScored?.Invoke(_scoreValue);
            OnDestroyed?.Invoke();
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            DamageCore();
        }

        if (other.tag == "Omni Shot")
        {
            DamageCore();
        }
    }
}
