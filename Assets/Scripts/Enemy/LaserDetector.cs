using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class LaserDetector : MonoBehaviour
{
    [SerializeField]
    [Range(0f,1f)]
    private float _effectiveness = 0.9f;
    [SerializeField]
    private bool _disableEffectiveness = false;
    private DodgingEnemy _parentEnemy = null;

    void Start()
    {
        _parentEnemy = transform.parent.GetComponent<DodgingEnemy>();
        if (_parentEnemy == null)
        {
            Debug.LogError("No Parent Enemy.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_disableEffectiveness == false)
        {
            float randomChance = Random.Range(0f, 1f);

            if (_effectiveness < randomChance)
            {
                return;
            } 
        }

        if (other.tag == "Laser")
        {
            _parentEnemy.ReceiveLaserToDodge(other.gameObject);
        }

        if (other.tag == "Omni Shot")
        {
            _parentEnemy.ReceiveLaserToDodge(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _parentEnemy.CheckDodgedLaser(other.gameObject);
        }

        if (other.tag == "Omni Shot")
        {
            _parentEnemy.CheckDodgedLaser(other.gameObject);
        }
    }
}
