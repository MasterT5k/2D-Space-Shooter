using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotSpeed = 5f;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    [SerializeField]
    private float _destroyDelay = 0.5f;
    private SpawnManager _spawnManager = null;
    private PoolManager _poolManager = null;

    void Start()
    {
        _poolManager = GameObject.Find("Pool Manager").GetComponent<PoolManager>();
        if (_poolManager == null)
        {
            Debug.LogError("Pool Manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("SpawnManager is NULL");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            _spawnManager.StartSpawning();
            gameObject.GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, _destroyDelay);
        }

        if (other.tag == "Omni Shot")
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            _spawnManager.StartSpawning();
            gameObject.GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, _destroyDelay);
        }
    }
}
