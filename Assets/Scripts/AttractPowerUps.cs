using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AttractPowerUps : MonoBehaviour
{
    private List<PowerUp> _powerUpsInRange = new List<PowerUp>();
    private bool _isObjectInRange = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.C) && _isObjectInRange == true)
        {
            for (int i = 0; i < _powerUpsInRange.Count; i++)
            {
                _powerUpsInRange[i].MoveToPlayer();
            }
        }

        if (Input.GetKeyUp(KeyCode.C) && _isObjectInRange == true)
        {
            for (int i = 0; i < _powerUpsInRange.Count; i++)
            {
                _powerUpsInRange[i].StopMovingToPlayer();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Power Up")
        {
            PowerUp powerUp = other.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                _powerUpsInRange.Add(powerUp);

                if (_isObjectInRange == false)
                {
                    _isObjectInRange = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Power Up")
        {
            PowerUp powerUp = other.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                powerUp.StopMovingToPlayer();
                _powerUpsInRange.Remove(powerUp);

                if (_powerUpsInRange.Count == 0)
                {
                    _isObjectInRange = false;
                }
            }
        }
    }
}
