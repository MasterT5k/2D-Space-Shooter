using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class BeamEmitter : MonoBehaviour
{
    [SerializeField]
    private int _startingHealth = 3;
    private int _currentHealth;
    [SerializeField]
    private GameObject _beamObj = null;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    private PoolManager _poolManager = null;

    void Start()
    {
        _poolManager = GameObject.Find("Pool Manager").GetComponent<PoolManager>();
        if (_poolManager == null)
        {
            Debug.LogError("Pool Manager is NULL");
        }

        _currentHealth = _startingHealth;
        _beamObj.GetComponent<LaserBeam>().AssignEnemyBeam();
        _beamObj.SetActive(false);
    }

    void Damage(int amount = -1)
    {
        _currentHealth += amount;
        if (_currentHealth <= 0)
        {
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = _poolManager.GetInactiveExplosion(explosionID);
            explosion.transform.position = transform.position;
            explosion.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    public void LaserBeams(bool turnOn)
    {
        if (turnOn == true)
        {
            _beamObj.SetActive(true);
        }
        else
        {
            _beamObj.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.ChangeLives();
            }

            Damage();
        }

        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            Damage();
        }

        if (other.tag == "Omni Shot")
        {
            Damage();
        }
    }
}
