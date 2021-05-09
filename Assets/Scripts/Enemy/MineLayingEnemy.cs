using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayingEnemy : Enemy
{
    [SerializeField]
    private GameObject _minePrefab = null;

    protected override void FireLaser()
    {
        float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
        _canFire = Time.time + fireRate;

        Instantiate(_minePrefab, _laserSpawn.position, Quaternion.identity);
    }
}
