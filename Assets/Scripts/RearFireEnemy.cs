using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearFireEnemy : Enemy
{
    [Header("Rear Fire Variables")]
    [SerializeField]
    private Transform _rearLaserSpawn = null;
    private bool _isPlayerBehind = false;

    protected override void Start()
    {
        base.Start();

        DeterminePlayerPosition();
    }

    protected override void Update()
    {
        DeterminePlayerPosition();

        base.Update();
    }

    protected override void FireLaser()
    {
        if (_isPlayerBehind == false)
        {
            base.FireLaser(); 
        }
        else
        {
            float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
            _canFire = Time.time + fireRate;

            PlayClip(_laserClip);

            Instantiate(_laserPrefab, _rearLaserSpawn.position, Quaternion.identity);
        }
    }

    void DeterminePlayerPosition()
    {
        float playerYAxis = _player.transform.position.y;
        float enemyYAxis = transform.position.y;

        if (playerYAxis >= enemyYAxis)
        {
            _isPlayerBehind = true;
        }
        else
        {
            _isPlayerBehind = false;
        }
    }
}
