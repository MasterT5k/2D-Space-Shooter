using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MineLayingEnemy : Enemy
{
    [SerializeField]
    private WeaponID _minePrefab = null;

    protected override void FireLaser()
    {
        float fireRate = Random.Range(_minFireDelay, _maxFireDelay);
        _canFire = Time.time + fireRate;

        int weaponType = _minePrefab.GetWeaponType();
        GameObject mine = GetWeapon(weaponType);
        if (mine != null)
        {
            mine.transform.position = _laserSpawns[0].position;
            mine.SetActive(true); 
        }
    }
}
