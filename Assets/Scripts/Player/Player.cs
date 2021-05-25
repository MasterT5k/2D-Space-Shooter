using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _upBounds = 0;
    [SerializeField]
    private float _downBounds = -3.8f;
    [SerializeField]
    private float _leftBounds = -11.3f;
    [SerializeField]
    private float _rightBounds = 11.3f;
    [SerializeField]
    private bool _wrapHorizontal = true;

    private float _baseSpeed;
    [SerializeField]
    private float _thrusterBoost = 2f;
    [SerializeField]
    private float _thrusterBurnLength = 5;
    private float _elapsedTime = 0;
    private bool _isThrusterActive = false;
    private bool _isThrusterDown = false;

    [Header("Shooting Settings")]
    [SerializeField]
    private float _fireRate = 0.25f;
    [SerializeField]
    private int _maxAmmo = 15;
    [SerializeField]
    private Transform[] _spawnPoints = null;
    [SerializeField]
    private WeaponID _laserPrefab = null;
    [SerializeField]
    private AudioClip _laserClip = null;
    private float _canFire;
    private int _currentAmmo;

    [Header("PowerUp Up Settings")]
    [SerializeField]
    private float _powerUpDuration = 5f;
    private bool _isTripleShotActive = false;
    private float _tripleShotDuration;

    [SerializeField]
    private WeaponID _omniShotPrefab = null;
    private bool _isOmniShotActive = false;
    private float _omniShotDuration;

    [SerializeField]
    [Range(1, 3)]
    private int _shieldStrength = 3;
    [SerializeField]
    private GameObject _shieldVisual = null;
    private bool _isShieldActive = false;
    private int _currentShieldStrength;
    private SpriteRenderer _shieldRenderer = null;
    private Color _fullShieldColor = Color.white;
    [SerializeField]
    private Color _secondShieldColor;
    [SerializeField]
    private Color _lastShieldColor;

    [SerializeField]
    private float _speedMultiplier = 2f;
    private bool _isSpeedBoostActive = false;
    private float _speedBoostDuration;

    [SerializeField]
    private WeaponID _homingMissilePrefab = null;
    [SerializeField]
    private int _numberOfMissiles = 4;
    private int _currentMissiles;
    private bool _isHomingMissileActive = false;

    [SerializeField]
    private float _fireDelayIncrease = 2f;
    private float _negativeEffectDuration;
    private bool _isNegativeEffectActive = false;

    [Header("Health Settings")]
    [SerializeField]
    [Range(1, 3)]
    private int _maxLives = 3;
    private int _lives;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    [SerializeField]
    private GameObject[] _engineFires = null;

    public static event Action OnCameraShake;
    public static event Action<int> OnUpdateLives;
    public static event Action<float, float> OnUpdateThuster;
    public static event Action<int, int> OnUpdateAmmo;
    public static event Action<int, int> OnUpdateMissiles;
    public static event Action<GameObject> OnRemoveTarget;
    public static event Action OnDeath;
    public static event Action<AudioClip> OnPlaySFX;
    public static event Func<int, GameObject> OnGetLaser;
    public static event Func<int, GameObject> OnGetWeapon;
    public static event Func<int, GameObject> OnGetExplosion;

    void OnEnable()
    {
        GameManager.OnGetPlayerObj += GivePlayerObj;
        Enemy.OnDamagePlayer += ChangeLives;
        Laser.OnDamagePlayer += ChangeLives;
        HomingMissile.OnDamagePlayer += ChangeLives;
        BeamEmitter.OnDamagePlayer += ChangeLives;
        LaserBeam.OnDamagePlayer += ChangeLives;
        Mine.OnDamagePlayer += ChangeLives;
        BossAI.OnDestroyed += GameWon;
    }

    void OnDisable()
    {
        GameManager.OnGetPlayerObj -= GivePlayerObj;
        Enemy.OnDamagePlayer -= ChangeLives;
        Laser.OnDamagePlayer -= ChangeLives;
        HomingMissile.OnDamagePlayer -= ChangeLives;
        BeamEmitter.OnDamagePlayer -= ChangeLives;
        LaserBeam.OnDamagePlayer -= ChangeLives;
        Mine.OnDamagePlayer -= ChangeLives;
        BossAI.OnDestroyed -= GameWon;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        transform.position = Vector2.zero;
        _baseSpeed = _speed;
        _currentAmmo = _maxAmmo;
        _lives = _maxLives;
        _tripleShotDuration = _powerUpDuration;
        _speedBoostDuration = _powerUpDuration;
        _omniShotDuration = _powerUpDuration;
        _negativeEffectDuration = _powerUpDuration;

        _shieldRenderer = _shieldVisual.GetComponent<SpriteRenderer>();
        _fullShieldColor = _shieldRenderer.color;
        _shieldVisual.SetActive(false);

        for (int i = 0; i < _engineFires.Length; i++)
        {
            _engineFires[i].SetActive(false);
        }

        OnUpdateLives?.Invoke(_lives);
        OnUpdateThuster?.Invoke(_elapsedTime, _thrusterBurnLength);
        OnUpdateAmmo?.Invoke(_currentAmmo, _maxAmmo);
        OnUpdateMissiles?.Invoke(_currentMissiles, _numberOfMissiles);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _isHomingMissileActive == true)
        {
            int weaponType = _homingMissilePrefab.GetWeaponType();
            GameObject missile = OnGetWeapon?.Invoke(weaponType);
            if (missile != null)
            {
                missile.transform.position = _spawnPoints[0].position;
                missile.SetActive(true);
                _currentMissiles--;
                if (_currentMissiles < 1)
                {
                    _currentMissiles = 0;
                    _isHomingMissileActive = false;
                }
                OnUpdateMissiles?.Invoke(_currentMissiles, _numberOfMissiles);
            }
        }

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire && _currentAmmo > 0)
        {
            FireLaser();
        }

        CalculateMovement();
    }

    private void CalculateMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isThrusterDown == false)
        {
            _speed += _thrusterBoost;
            _isThrusterActive = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _baseSpeed;
            _isThrusterActive = false;
        }

        if (_isThrusterActive == true)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > _thrusterBurnLength)
            {
                _elapsedTime = _thrusterBurnLength;
                _isThrusterDown = true;
                _isThrusterActive = false;
                _speed = _baseSpeed;
            }

            OnUpdateThuster?.Invoke(_elapsedTime, _thrusterBurnLength);
        }
        else if (_isThrusterActive == false && _elapsedTime > 0)
        {
            _elapsedTime -= Time.deltaTime;

            if (_elapsedTime < 0)
            {
                _elapsedTime = 0;
                _isThrusterDown = false;
            }

            OnUpdateThuster?.Invoke(_elapsedTime, _thrusterBurnLength);
        }

        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(hInput, vInput);

        if (direction.x != 0 || direction.y != 0)
        {
            if (_isSpeedBoostActive == true)
            {
                transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * _speed * Time.deltaTime);
            }
        }

        transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, _downBounds, _upBounds));

        if (transform.position.x > _rightBounds)
        {
            if (_wrapHorizontal == true)
            {
                transform.position = new Vector2(_leftBounds, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(_rightBounds, transform.position.y);
            }
        }
        else if (transform.position.x < _leftBounds)
        {
            if (_wrapHorizontal == true)
            {
                transform.position = new Vector2(_rightBounds, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(_leftBounds, transform.position.y);
            }
        }
    }

    void FireLaser()
    {
        GameObject weapon;
        if (_isNegativeEffectActive == true || _isOmniShotActive == true)
        {
            _canFire = Time.time + _fireRate + _fireDelayIncrease;
        }
        else
        {
            _canFire = Time.time + _fireRate;
        }

        PlayClip(_laserClip);

        if (_isTripleShotActive == true)
        {
            int weaponType = _laserPrefab.GetWeaponType();
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                weapon = OnGetLaser?.Invoke(weaponType);
                if (weapon != null)
                {
                    weapon.transform.position = _spawnPoints[i].position;
                    weapon.SetActive(true); 
                }
            }
        }
        else if (_isOmniShotActive == true)
        {
            int weaponType = _omniShotPrefab.GetWeaponType();
            weapon = OnGetWeapon?.Invoke(weaponType);
            if (weapon != null)
            {
                weapon.transform.position = transform.position;
                weapon.SetActive(true); 
            }
        }
        else
        {
            int weaponType = _laserPrefab.GetWeaponType();
            weapon = OnGetLaser?.Invoke(weaponType);
            if (weapon != null)
            {
                weapon.transform.position = _spawnPoints[0].position;
                weapon.SetActive(true);
                _currentAmmo--;
                OnUpdateAmmo?.Invoke(_currentAmmo, _maxAmmo);
            }
        }
    }

    public void ChangeLives(int amount)
    {
        if (_isShieldActive == true && amount < 0)
        {
            _currentShieldStrength--;

            if (_currentShieldStrength > 0)
            {
                ChangeShield(_currentShieldStrength);
                return;
            }

            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        if (amount < 0)
        {
            OnCameraShake?.Invoke();
        }

        _lives += amount;

        if (_lives > _maxLives)
        {
            _lives = _maxLives;
        }
        else if (_lives < 0)
        {
            _lives = 0;
        }

        OnUpdateLives?.Invoke(_lives);

        if (_lives == 3)
        {
            if (_engineFires[0].activeInHierarchy == true)
            {
                _engineFires[0].SetActive(false);
            }

            if (_engineFires[1].activeInHierarchy == true)
            {
                _engineFires[1].SetActive(false);
            }
        }
        else if (_lives == 2)
        {
            _engineFires[0].SetActive(true);

            if (_engineFires[1].activeInHierarchy == true)
            {
                _engineFires[1].SetActive(false);
            }
        }
        else if (_lives == 1)
        {
            _engineFires[1].SetActive(true);

            if (_engineFires[0].activeInHierarchy == false)
            {
                _engineFires[0].SetActive(true);
            }
        }
        else if (_lives < 1)
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            OnRemoveTarget?.Invoke(gameObject);
            OnDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void TripleShotActivate()
    {
        if (_isTripleShotActive == false)
        {
            _isTripleShotActive = true;
            StartCoroutine(TripleShotPowerDownRoutine());
        }
        else
        {
            _tripleShotDuration += _powerUpDuration;
        }

        if (_isOmniShotActive == true)
        {
            _isOmniShotActive = false;
            _omniShotDuration = 0;
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while(_tripleShotDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _tripleShotDuration--;
        }

        _tripleShotDuration = _powerUpDuration;
        _isTripleShotActive = false;
    }

    public void SpeedBoostActivate()
    {
        if (_isSpeedBoostActive == false)
        {
            _isSpeedBoostActive = true;
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }
        else
        {
            _speedBoostDuration += _powerUpDuration;
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while (_speedBoostDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _speedBoostDuration--;
        }

        _speedBoostDuration = _powerUpDuration;
        _isSpeedBoostActive = false;
    }

    public void ShieldActivate()
    {
        if (_isShieldActive == false)
        {
            _isShieldActive = true;
            _shieldVisual.SetActive(true);
        }

        _currentShieldStrength = _shieldStrength;
        ChangeShield(_currentShieldStrength);
    }

    private void ChangeShield(int shieldStrength)
    {
        if (shieldStrength == 3)
        {
            _shieldRenderer.color = _fullShieldColor;
        }
        else if (shieldStrength == 2)
        {
            _shieldRenderer.color = _secondShieldColor;
        }
        else
        {
            _shieldRenderer.color = _lastShieldColor;
        }
    }

    public void PlayClip(AudioClip clip)
    {
        OnPlaySFX?.Invoke(clip);
    }

    public void ChangeAmmo()
    {
        _currentAmmo = _maxAmmo;

        OnUpdateAmmo?.Invoke(_currentAmmo, _maxAmmo);
    }

    public void ActivateHomingMissiles()
    {
        _isHomingMissileActive = true;
        _currentMissiles = _numberOfMissiles;
        OnUpdateMissiles?.Invoke(_currentMissiles, _numberOfMissiles);
    }

    public void OmniShotActivate()
    {
        if (_isOmniShotActive == false)
        {
            _isOmniShotActive = true;
            StartCoroutine(OmniShotPowerDownRoutine());
        }
        else
        {
            _omniShotDuration += _powerUpDuration;
        }

        if (_isTripleShotActive == true)
        {
            _isTripleShotActive = false;
            _tripleShotDuration = 0;
        }
    }

    IEnumerator OmniShotPowerDownRoutine()
    {
        while (_omniShotDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _omniShotDuration--;
        }

        _omniShotDuration = _powerUpDuration;
        _isOmniShotActive = false;
    }

    public void NegativeEffectActivate()
    {
        if (_isNegativeEffectActive == false)
        {
            _canFire += _fireDelayIncrease;
            _isNegativeEffectActive = true;
            StartCoroutine(NegativeEffectPowerDownRoutine());
        }
        else
        {
            _negativeEffectDuration += _powerUpDuration;
        }
    }

    IEnumerator NegativeEffectPowerDownRoutine()
    {
        while (_negativeEffectDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _negativeEffectDuration--;
        }

        _negativeEffectDuration = _powerUpDuration;
        _isNegativeEffectActive = false;
    }

    private GameObject GivePlayerObj()
    {
        return this.gameObject;
    }

    private void GameWon()
    {
        this.enabled = false;
    }
}
