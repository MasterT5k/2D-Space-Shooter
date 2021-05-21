﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _enemyID = 0;

    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 4f;
    private float _baseSpeed;
    [SerializeField]
    protected float _horizontalLimits = 8;
    [SerializeField]
    private float _verticalLimits = -8f;

    [Header("Death Settings")]
    [SerializeField]
    private float _destroyDelay = 2.6f;
    [SerializeField]
    private AudioClip _explosionClip = null;
    protected bool _isDead = false;

    [Header("Shooting Settings")]
    [SerializeField]
    protected WeaponID _laserPrefab;
    [SerializeField]
    protected Transform[] _laserSpawns;
    [SerializeField]
    protected AudioClip _laserClip = null;
    [SerializeField]
    protected int _minFireDelay = 3, _maxFireDelay = 8;
    protected float _canFire;

    [Header("Shield Settings")]
    [SerializeField]
    [Range(1, 3)]
    private int _shieldStrength = 1;
    [SerializeField]
    [Range(0, 1)]
    private float _chanceOfShield = 0.5f;
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

    private bool _isReused = false;
    protected Player _player = null;
    private Animator _anim = null;
    protected PoolManager _poolManager = null;

    void OnEnable()
    {
        if (_isReused == true)
        {
            _isDead = false;
            ShieldChance(_chanceOfShield);
        }
    }

    protected virtual void Start()
    {
        _poolManager = GameObject.Find("Pool Manager").GetComponent<PoolManager>();
        if (_poolManager == null)
        {
            Debug.LogError("Pool Manager is NULL");
        }
        _baseSpeed = _speed;
        Init();
        _isReused = true;
    }

    protected virtual void Update()
    {
        if (Time.time > _canFire && _isDead == false)
        {
            FireLaser();
        }

        CalulateMovement();
    }

    void Init()
    {
        _shieldRenderer = _shieldVisual.GetComponent<SpriteRenderer>();
        _fullShieldColor = _shieldRenderer.color;
        ShieldChance(_chanceOfShield);

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = GetComponentInChildren<Animator>();
        if (_anim == null)
        {
            Debug.Log("Animator is NULL");
        }
    }

    protected virtual void CalulateMovement()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < _verticalLimits)
        {
            Respawn();
        }
    }

    protected virtual void Respawn()
    {
        float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
        Vector2 spawnPoint = new Vector2(randomX, -_verticalLimits);
        transform.position = spawnPoint;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Omni Shot")
        {
            if (_isShieldActive == true)
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

            if (_player != null)
            {
                _player.AddScore();
            }

            if (_isDead == false)
            {
                PlayClip(_explosionClip);
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            gameObject.tag = "Untagged";
            _speed = 0;
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(DisableRoutine());
        }

        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);

            if (_isShieldActive == true)
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

            if (_player != null)
            {
                _player.AddScore();
            }

            if (_isDead == false)
            {
                PlayClip(_explosionClip);
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            gameObject.tag = "Untagged";
            _speed = 0;
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(DisableRoutine());
        }

        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.ChangeLives();
            }

            if (_isShieldActive == true)
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

            if (_isDead == false)
            {
                PlayClip(_explosionClip);
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            gameObject.tag = "Untagged";
            _speed = 0;
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(DisableRoutine());
        }
    }

    protected virtual void FireLaser()
    {
        float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
        _canFire = Time.time + fireRate;

        PlayClip(_laserClip);

        GameObject enemyLaser;
        for (int i = 0; i < _laserSpawns.Length; i++)
        {
            int weaponType = _laserPrefab.GetWeaponType();
            enemyLaser = _poolManager.GetInactiveLaser(weaponType);
            enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
            enemyLaser.transform.position = _laserSpawns[i].position;            
            enemyLaser.SetActive(true); 
        }
    }

    protected void PlayClip(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(clip);
    }

    protected void ShieldChance(float percentage)
    {
        float randomChance = Random.Range(0f, 1f);

        if (percentage > randomChance)
        {
            _isShieldActive = true;
            _shieldVisual.SetActive(true);
            _currentShieldStrength = _shieldStrength;
            ChangeShield(_currentShieldStrength);
        }
        else
        {
            _shieldVisual.SetActive(false);
        }
    }

    protected void ChangeShield(int shieldStrength)
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

    public int GetEnemyID()
    {
        return _enemyID;
    }

    IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(_destroyDelay);
        _speed = _baseSpeed;
        _anim.SetTrigger("Destroyed");
        gameObject.GetComponent<Collider2D>().enabled = true;
        _poolManager.PutEnemyInHolder(this.gameObject);
        gameObject.tag = "Enemy";
        gameObject.SetActive(false);
    }
}
