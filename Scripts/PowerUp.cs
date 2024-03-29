﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private AudioClip _powerUpClip = null;

    [SerializeField]
    private PowerUpType _powerUpType = PowerUpType.TripleShot;
    private enum PowerUpType
    {
        TripleShot,
        SpeedBoost,
        Shield
    }

    void Update()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.PlayClip(_powerUpClip);
                switch (_powerUpType)
                {
                    case PowerUpType.TripleShot:
                        player.TripleShotActivate();
                        break;
                    case PowerUpType.SpeedBoost:
                        player.SpeedBoostActivate();
                        break;
                    case PowerUpType.Shield:
                        player.ShieldActivate();
                        break;
                    default:
                        Debug.Log("Power Up ID Not Found");
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
