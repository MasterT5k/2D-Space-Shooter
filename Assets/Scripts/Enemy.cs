using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    private Player _player = null;
    private Animator _anim = null;
    [SerializeField]
    private AudioClip _explosionClip = null;
    private AudioSource _source = null;
    private bool _isDead = false;

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
}
