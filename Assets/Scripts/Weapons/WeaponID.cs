using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponID : MonoBehaviour
{
    [SerializeField]
    private WeaponType _weaponType;    
    private enum WeaponType
    {
        PlayerLaser,
        PlayerMissile,
        PlayerOmniShot,
        PlayerMine,
        EnemyLaser,
        EnemyMissile,
        EnemyOmniShot,
        EnemyMine
    }

    public int GetWeaponType()
    {
        return (int)_weaponType;
    }
}
