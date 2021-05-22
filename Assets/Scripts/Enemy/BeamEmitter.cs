using System;
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
    private int _scoreValue = 20;
    [SerializeField]
    private GameObject _beamObj = null;
    [SerializeField]
    private Explosion _explosionPrefab = null;
    private Collider2D _collider = null;

    public static event Action<GameObject> OnActivated;
    public static event Action<GameObject> OnDestroyed;
    public static event Action<int> OnDamagePlayer;
    public static event Action<int> OnScored;
    public static event Func<int, GameObject> OnGetExplosion;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        _collider.enabled = false;
        OnActivated?.Invoke(this.gameObject);
        _currentHealth = _startingHealth;
    }

    void OnDisable()
    {
        _beamObj.SetActive(false);
    }

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
            int explosionID = _explosionPrefab.GetExplosionID();
            GameObject explosion = OnGetExplosion?.Invoke(explosionID);
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.SetActive(true);
            }
            OnScored?.Invoke(_scoreValue);
            OnDestroyed?.Invoke(this.gameObject);
            gameObject.SetActive(false);
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

    public void ActivateCollider()
    {
        _collider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            OnDamagePlayer?.Invoke(-1);
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
