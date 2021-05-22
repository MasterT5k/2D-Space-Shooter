using System;
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

    public static event Action OnStartSpawning;
    public static event Func<int, GameObject> OnGetExplosion;

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
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.SetActive(true);
            }
            OnStartSpawning?.Invoke();
            gameObject.GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, _destroyDelay);
        }

        if (other.tag == "Omni Shot")
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.SetActive(true);
            }
            OnStartSpawning?.Invoke();
            gameObject.GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, _destroyDelay);
        }
    }
}
