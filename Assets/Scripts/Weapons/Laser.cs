using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    [SerializeField]
    private float _yDestroyDistance = 12f;
    [SerializeField]
    private int _damage = -1;
    private bool _isEnemyLaser = false;

    public static event Action<int> OnDamagePlayer;

    void OnDisable()
    {
        transform.position = transform.parent.position;
        _isEnemyLaser = false;
    }

    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _yDestroyDistance)
        {
            gameObject.SetActive(false);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -_yDestroyDistance)
        {
            gameObject.SetActive(false);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            OnDamagePlayer?.Invoke(_damage);
            gameObject.SetActive(false);
        }
    }
}
