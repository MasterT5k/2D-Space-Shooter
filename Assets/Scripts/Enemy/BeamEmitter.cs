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
    private GameObject _explosionPrefab = null;

    void Start()
    {
        _currentHealth = _startingHealth;
        _beamObj.GetComponent<LaserBeam>().AssignEnemyBeam();
        _beamObj.SetActive(false);
    }

    void Damage(int amount = -1)
    {
        _currentHealth += amount;
        if (_currentHealth <= 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
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
            Destroy(other.gameObject);
            Damage();
        }

        if (other.tag == "Omni Shot")
        {
            Damage();
        }
    }
}
