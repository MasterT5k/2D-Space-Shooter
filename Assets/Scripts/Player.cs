using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
    private Transform _spawnPoint = null;
    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private AudioClip _laserClip = null;
    private float _canFire;
    private int _currentAmmo;

    [Header("PowerUp Up Settings")]
    [SerializeField]
    private float _powerUpDuration = 5f;
    [SerializeField]
    private GameObject _tripleLaserPrefab = null;
    private bool _isTripleShotActive = false;
    private float _tripleShotDuration;

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
    private GameObject _homingMissilePrefab = null;
    [SerializeField]
    private int _numberOfMissiles = 4;
    private int _currentMissiles;
    private bool _isHomingMissileActive = false;

    [Header("Health Settings")]
    [SerializeField]
    [Range(1, 3)]
    private int _maxLives = 3;
    private int _lives;
    [SerializeField]
    private GameObject[] _engineFires = null;

    private int _score = 0;
    private AudioSource _source = null;
    private SpawnManager _spawnManager = null;
    private UIManager _uIManager = null;


    void Start()
    {

        Init();

    }

    void Init()
    {
        transform.position = new Vector2(0, 0);

        _baseSpeed = _speed;
        _currentAmmo = _maxAmmo;
        _lives = _maxLives;
        _tripleShotDuration = _powerUpDuration;
        _speedBoostDuration = _powerUpDuration;


        _shieldRenderer = _shieldVisual.GetComponent<SpriteRenderer>();
        _shieldVisual.SetActive(false);
        _source = GetComponent<AudioSource>();

        for (int i = 0; i < _engineFires.Length; i++)
        {
            _engineFires[i].SetActive(false);
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("SpawnManager is NULL");
        }

        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.Log("UIManager is NULL");
        }

        _uIManager.UpdateScore(_score);
        _uIManager.UpdateLivesImage(_lives);
        _uIManager.UpdateThrusterBar(_elapsedTime, _thrusterBurnLength);
        _uIManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _isHomingMissileActive == true)
        {
            GameObject missile = Instantiate(_homingMissilePrefab, _spawnPoint.position, Quaternion.identity);
            missile.GetComponent<HomingMissile>().AssignTarget();
            _currentMissiles--;
            if (_currentMissiles < 1)
            {
                _currentMissiles = 0;
                _isHomingMissileActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _currentAmmo > 0)
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

            _uIManager.UpdateThrusterBar(_elapsedTime, _thrusterBurnLength);
        }
        else if (_isThrusterActive == false && _elapsedTime > 0)
        {
            _elapsedTime -= Time.deltaTime;

            if (_elapsedTime < 0)
            {
                _elapsedTime = 0;
                _isThrusterDown = false;
            }

            _uIManager.UpdateThrusterBar(_elapsedTime, _thrusterBurnLength);
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
        _canFire = Time.time + _fireRate;
        PlayClip(_laserClip);

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleLaserPrefab, _spawnPoint.position, Quaternion.identity);

        }
        else
        {
            Instantiate(_laserPrefab, _spawnPoint.position, Quaternion.identity);
            _currentAmmo--;
            _uIManager.UpdateAmmo(_currentAmmo, _maxAmmo);
        }
    }

    public void ChangeLives(int amount = -1)
    {
        if (_isShieldActive == true && amount < 0)
        {
            _currentShieldStrength--;

            if (_currentShieldStrength > 0)
            {
                ChangeShield(_currentShieldStrength);
                Debug.Log("Shield Hit!");
                return;
            }

            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            Debug.Log("Shield Off!");
            return;
        }

        if (amount < 0)
        {
            CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
            if (cameraShake != null)
            {
                cameraShake.ShakeCamera();
            }
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

        _uIManager.UpdateLivesImage(_lives);

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
            Debug.Log("BOOM!");
            _spawnManager.StopSpawning();
            Destroy(gameObject);
        }
    }

    public void AddScore()
    {
        _score += 10;
        _uIManager.UpdateScore(_score);
    }

    public void TripleShotActivate()
    {
        if (_isTripleShotActive == false)
        {
            _isTripleShotActive = true;
            Debug.Log("Triple Shot On!");
            StartCoroutine(TripleShotPowerDownRoutine());
        }
        else
        {
            _tripleShotDuration += _powerUpDuration;
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
        Debug.Log("Triple Shot Off!");
    }

    public void SpeedBoostActivate()
    {
        if (_isSpeedBoostActive == false)
        {
            _isSpeedBoostActive = true;
            Debug.Log("Speed Boost On!");
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
        Debug.Log("Speed Boost Off!");
    }

    public void ShieldActivate()
    {
        if (_isShieldActive == false)
        {
            _isShieldActive = true;
            _shieldVisual.SetActive(true);
            Debug.Log("Shield On!");
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
        _source.PlayOneShot(clip);
    }

    public void ChangeAmmo()
    {
        _currentAmmo = _maxAmmo;

        _uIManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    public void ActivateHomingMissiles()
    {
        _isHomingMissileActive = true;
        _currentMissiles = _numberOfMissiles;
    }
}
