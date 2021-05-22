using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private float _verticalLimit = -5.5f;
    [SerializeField]
    private AudioClip _powerUpClip = null;
    [SerializeField]
    private int _healAmount = 1;
    private Transform _playerTransform = null;
    private bool _isMovingToPlayer = false;

    public static event Action<AudioClip> OnPlaySFX;
    public static event Func<GameObject> OnGetPlayerObj;
    public static event Func<Player> OnGetPlayerScript;

    [SerializeField]
    private PowerUpType _powerUpType = PowerUpType.TripleShot;
    private enum PowerUpType
    {
        TripleShot,
        SpeedBoost,
        Shield,
        Ammo,
        Health,
        Missile,
        OmniShot,
        Negative
    }

    private void OnDisable()
    {
        transform.position = transform.parent.position;
    }

    void Start()
    {
        GameObject playerObj = OnGetPlayerObj?.Invoke();
        if (playerObj != null)
        {
            _playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (_isMovingToPlayer == false)
        {
            transform.Translate(Vector2.down * _speed * Time.deltaTime);

            if (transform.position.y < _verticalLimit)
            {
                gameObject.SetActive(false);
            }
        }
        else if (_playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, _speed * Time.deltaTime);
        }
        else
        {
            _isMovingToPlayer = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = OnGetPlayerScript?.Invoke();
            if (player != null)
            {
                OnPlaySFX?.Invoke(_powerUpClip);
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
                    case PowerUpType.Ammo:
                        player.ChangeAmmo();
                        break;
                    case PowerUpType.Health:
                        player.ChangeLives(_healAmount);
                        break;
                    case PowerUpType.Missile:
                        player.ActivateHomingMissiles();
                        break;
                    case PowerUpType.OmniShot:
                        player.OmniShotActivate();
                        break;
                    case PowerUpType.Negative:
                        player.ChangeLives(-_healAmount);
                        player.NegativeEffectActivate();
                        break;
                    default:
                        break;
                }
            }
            gameObject.SetActive(false);
        }
    }

    public void MoveToPlayer()
    {
        if (_isMovingToPlayer == false)
        {
            _isMovingToPlayer = true;
        }
    }

    public void StopMovingToPlayer()
    {
        if (_isMovingToPlayer == true)
        {
            _isMovingToPlayer = false;
        }
    }

    public int GetPowerUpType()
    {
        return (int)_powerUpType;
    }
}
