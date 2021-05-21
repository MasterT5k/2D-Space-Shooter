using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayingEnemy : Enemy
{
    [SerializeField]
    private WeaponID _minePrefab = null;

    protected override void FireLaser()
    {
        float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
        _canFire = Time.time + fireRate;

        //Instantiate(_minePrefab, _laserSpawns[0].position, Quaternion.identity);
        int weaponType = _minePrefab.GetWeaponType();
        GameObject mine = _poolManager.GetInactiveWeapon(weaponType);
        mine.transform.position = _laserSpawns[0].position;
        mine.SetActive(true);
    }
}
