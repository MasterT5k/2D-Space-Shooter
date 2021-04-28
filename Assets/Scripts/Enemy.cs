using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private float _horizontalLimits = 8;
    [SerializeField]
    private float _verticalLimits = -8f;

    [Header("Death Settings")]
    [SerializeField]
    private float _destroyDelay = 2.6f;
    [SerializeField]
    private AudioClip _explosionClip = null;
    private bool _isDead = false;

    [Header("Shooting Settings")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Transform _laserSpawn;
    [SerializeField]
    private AudioClip _laserClip = null;
    [SerializeField]
    private int _minFireDelay = 3, _maxFireDelay = 8;
    private float _canFire;

    private AudioSource _source = null;
    private Player _player = null;
    private Animator _anim = null;

    void Start()
    {
        _source = GetComponent<AudioSource>();

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = GetComponentInChildren<Animator>();
        if (_anim == null)
        {
            Debug.Log("Animator is NULL");
        }
    }

    void Update()
    {
        if (Time.time > _canFire && _isDead == false)
        {
            FireLaser();
        }

        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < _verticalLimits)
        {
            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnPoint = new Vector2(randomX, -_verticalLimits);
            transform.position = spawnPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            
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
            Destroy(gameObject, _destroyDelay);
        }

        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.ChangeLives();
            }

            if (_isDead == false)
            {
                _source.Play();
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            gameObject.tag = "Untagged";
            _speed = 0;
            Destroy(gameObject, _destroyDelay);
        }
    }

    void FireLaser()
    {
        float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
        _canFire = Time.time + fireRate;

        PlayClip(_laserClip);

        GameObject enemyLaser = Instantiate(_laserPrefab, _laserSpawn.position, Quaternion.identity);

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    void PlayClip(AudioClip clip)
    {
        _source.PlayOneShot(clip);
    }
}
