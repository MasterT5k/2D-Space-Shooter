using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
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
    [SerializeField]
    private Transform _spawnPoint = null;

    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private GameObject _tripleLaserPrefab = null;
    [SerializeField]
    private AudioClip _laserClip = null;
    private AudioSource _source = null;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score = 0;

    private bool _isTripleShotActive = false;
    private float _tripleShotDuration = 5f;

    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedMultiplier = 2f;
    private float _speedBoostDuration = 5f;

    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisual = null;
    
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire;

    [SerializeField]
    private GameObject[] _engineFires = null;

    private SpawnManager _spawnManager = null;
    private UIManager _uIManager = null;


    void Start()
    {
        transform.position = new Vector2(0, 0);

        _shieldVisual.SetActive(false);
        _source = GetComponent<AudioSource>();
        if (_source == null)
        {
            Debug.Log("Audio Source is NULL");
        }

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        CalculateMovement();
    }

    private void CalculateMovement()
    {
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
        }
    }

    public void ChangeLives(int amount)
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            Debug.Log("Shield Off!");
            return;
        }

        _lives += amount;
        _uIManager.UpdateLivesImage(_lives);

        if (_lives == 2)
        {
            _engineFires[0].SetActive(true);
        }

        if (_lives == 1)
        {
            _engineFires[1].SetActive(true);
        }

        if (_lives < 1)
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
            _tripleShotDuration += 5f;
        }        
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while(_tripleShotDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _tripleShotDuration--;
        }

        _tripleShotDuration = 5f;
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
            _speedBoostDuration += 5f;
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while (_speedBoostDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            _speedBoostDuration--;
        }

        _speedBoostDuration = 5f;
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
    }

    public void PlayClip(AudioClip clip)
    {
        _source.PlayOneShot(clip);
    }
}
