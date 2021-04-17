using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private float _horizontalLimits = 8;
    [SerializeField]
    private float _verticalLimits = -8f;
    private Player _player = null;
    private Animator _anim = null;
    [SerializeField]
    private AudioClip _explosionClip = null;
    private AudioSource _source = null;
    private bool _isDead = false;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Transform _laserSpawn;
    [SerializeField]
    private AudioClip _laserClip = null;
    [SerializeField]
    private int _minFireDelay = 3, _maxFireDelay = 8;
    private float _canFire;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null)
        {
            Debug.Log("Audio Source is NULL");
        }
        else
        {
            _source.clip = _explosionClip;
        }

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

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-8f, 8f);
            Vector2 spawnPoint = new Vector2(randomX, 8f);
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
                _source.Play();
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            _speed = 0;
            Destroy(gameObject, 2.6f);
        }

        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.ChangeLives(-1);
            }

            if (_isDead == false)
            {
                _source.Play();
                _isDead = true;
            }

            _anim.SetTrigger("Destroyed");
            _speed = 0;
            Destroy(gameObject, 2.6f);
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
