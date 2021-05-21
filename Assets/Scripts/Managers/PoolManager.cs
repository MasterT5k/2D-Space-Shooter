using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("Enemy Pool Settings")]
    [SerializeField]
    private GameObject[] _enemyPrefabs = null;
    [SerializeField]
    private int _enemiesToPool = 10;
    [SerializeField]
    private Transform _enemyHolder = null;
    private List<GameObject> _enemyPool = new List<GameObject>();

    [Header("Power Up Pool Settings")]
    [SerializeField]
    private GameObject[] _powerUpPrefabs = null;
    [SerializeField]
    private int _powerUpsToPool = 2;
    [SerializeField]
    private Transform _powerUpHolder = null;
    private List<GameObject> _powerUpPool = new List<GameObject>();

    [Header("Laser Pool Settings")]
    [SerializeField]
    private GameObject[] _laserPrefabs = null;
    [SerializeField]
    private int _lasersToPool = 20;
    [SerializeField]
    private Transform _laserHolder = null;
    [SerializeField]
    private List<GameObject> _laserPool = new List<GameObject>();

    [Header("Weapon Pool Settings")]
    [SerializeField]
    private GameObject[] _weaponPrefabs = null;
    [SerializeField]
    private int _weaponsToPool = 10;
    [SerializeField]
    private Transform _weaponHolder = null;
    private List<GameObject> _weaponPool = new List<GameObject>();

    [Header("Explosion Pool Settings")]
    [SerializeField]
    private GameObject[] _explosionPrefabs = null;
    [SerializeField]
    private int _explosionsToPool = 10;
    [SerializeField]
    private Transform _explosionHolder = null;
    private List<GameObject> _explosionPool = new List<GameObject>();

    void Start()
    {
        _enemyPool = GeneratePool(_enemyPool, _enemyPrefabs, _enemiesToPool, _enemyHolder);
        _powerUpPool = GeneratePool(_powerUpPool, _powerUpPrefabs, _powerUpsToPool, _powerUpHolder);
        _laserPool = GeneratePool(_laserPool, _laserPrefabs, _lasersToPool, _laserHolder);
        _weaponPool = GeneratePool(_weaponPool, _weaponPrefabs, _weaponsToPool, _weaponHolder);
        _explosionPool = GeneratePool(_explosionPool, _explosionPrefabs, _explosionsToPool, _explosionHolder);
    }

    private List<GameObject> GeneratePool(List<GameObject> objPool, GameObject[] objPrefabs, int baseSpawnCount, Transform objHolder)
    {
        for (int i = 0; i < objPrefabs.Length; i++)
        {
            GameObject objPrefab = objPrefabs[i];
            for (int n = 0; n < baseSpawnCount; n++)
            {
                GameObject obj = Instantiate(objPrefab, objHolder);
                objPool.Add(obj);
                obj.SetActive(false);
            }
        }
        return objPool;
    }

    public GameObject GetInactiveEnemy(int enemyID)
    {
        GameObject enemyToSpawn = null;
        for (int i = 0; i < _enemyPool.Count; i++)
        {
            if (_enemyPool[i].activeInHierarchy == false)
            {
                Enemy enemy = _enemyPool[i].GetComponent<Enemy>();
                if (enemyID == enemy.GetEnemyID())
                {
                    enemyToSpawn = _enemyPool[i];
                    return enemyToSpawn;
                }
            }
        }

        if (enemyToSpawn == null)
        {
            for (int i = 0; i < _enemyPrefabs.Length; i++)
            {
                Enemy enemy = _enemyPrefabs[i].GetComponent<Enemy>();
                if (enemyID == enemy.GetEnemyID())
                {
                    GameObject enemyPrefab = _enemyPrefabs[i];
                    GameObject enemyObj = Instantiate(enemyPrefab, _enemyHolder);
                    _enemyPool.Add(enemyObj);
                    enemyToSpawn = enemyObj;
                    Debug.Log("Creating new Enemy.");
                    return enemyToSpawn;
                }
            }
        }
        return null;
    }

    public GameObject GetInactivePowerUp(int powerUpType)
    {
        GameObject powerUpToSpawn = null;
        for (int i = 0; i < _powerUpPool.Count; i++)
        {
            if (_powerUpPool[i].activeInHierarchy == false)
            {
                PowerUp powerUp = _powerUpPool[i].GetComponent<PowerUp>();
                if (powerUpType == powerUp.GetPowerUpType())
                {
                    powerUpToSpawn = _powerUpPool[i];
                    return powerUpToSpawn;
                }
            }
        }

        if (powerUpToSpawn == null)
        {
            for (int i = 0; i < _powerUpPrefabs.Length; i++)
            {
                PowerUp powerUp = _powerUpPrefabs[i].GetComponent<PowerUp>();
                if (powerUpType == powerUp.GetPowerUpType())
                {
                    GameObject powerUpPrefab = _powerUpPrefabs[i];
                    GameObject powerUpObj = Instantiate(powerUpPrefab, _powerUpHolder);
                    _powerUpPool.Add(powerUpObj);
                    powerUpToSpawn = powerUpObj;
                    Debug.Log("Creating new Power Up.");
                    return powerUpToSpawn;
                }
            }
        }
        return null;
    }

    public GameObject GetInactiveLaser(int laserType)
    {
        GameObject laserToSpawn = null;
        for (int i = 0; i < _laserPool.Count; i++)
        {
            if (_laserPool[i].activeInHierarchy == false)
            {
                WeaponID laser = _laserPool[i].GetComponent<WeaponID>();
                if (laserType == laser.GetWeaponType())
                {
                    laserToSpawn = _laserPool[i];
                    return laserToSpawn;
                }
            }
        }

        if (laserToSpawn == null)
        {
            for (int i = 0; i < _laserPrefabs.Length; i++)
            {
                WeaponID laser = _laserPrefabs[i].GetComponent<WeaponID>();
                if (laserType == laser.GetWeaponType())
                {
                    GameObject laserPrefab = _laserPrefabs[i];
                    GameObject laserObj = Instantiate(laserPrefab, _laserHolder);
                    _laserPool.Add(laserObj);
                    laserToSpawn = laserObj;
                    Debug.Log("Creating new Laser.");
                    return laserToSpawn;
                }
            }
        }
        return null;
    }

    public GameObject GetInactiveWeapon(int weaponType)
    {
        GameObject weaponToSpawn = null;
        for (int i = 0; i < _weaponPool.Count; i++)
        {
            if (_weaponPool[i].activeInHierarchy == false)
            {
                WeaponID laser = _weaponPool[i].GetComponent<WeaponID>();
                if (weaponType == laser.GetWeaponType())
                {
                    weaponToSpawn = _weaponPool[i];
                    return weaponToSpawn;
                }
            }
        }

        if (weaponToSpawn == null)
        {
            for (int i = 0; i < _weaponPrefabs.Length; i++)
            {
                WeaponID weapon = _weaponPrefabs[i].GetComponent<WeaponID>();
                if (weaponType == weapon.GetWeaponType())
                {
                    GameObject weaponPrefab = _weaponPrefabs[i];
                    GameObject weaponObj = Instantiate(weaponPrefab, _weaponHolder);
                    _weaponPool.Add(weaponObj);
                    weaponToSpawn = weaponObj;
                    Debug.Log("Creating new Weapon.");
                    return weaponToSpawn;
                }
            }
        }
        return null;
    }

    public GameObject GetInactiveExplosion(int explosionID)
    {
        GameObject explosionToSpawn = null;
        for (int i = 0; i < _explosionPool.Count; i++)
        {
            if (_explosionPool[i].activeInHierarchy == false)
            {
                Explosion explosion = _explosionPool[i].GetComponent<Explosion>();
                if (explosionID == explosion.GetExplosionID())
                {
                    explosionToSpawn = _explosionPool[i];
                    return explosionToSpawn;
                }
            }
        }

        if (explosionToSpawn == null)
        {
            for (int i = 0; i < _explosionPrefabs.Length; i++)
            {
                Explosion explosion = _explosionPrefabs[i].GetComponent<Explosion>();
                if (explosionID == explosion.GetExplosionID())
                {
                    GameObject explosionPrefab = _explosionPrefabs[i];
                    GameObject explosionObj = Instantiate(explosionPrefab, _explosionHolder);
                    _explosionPool.Add(explosionObj);
                    explosionToSpawn = explosionObj;
                    Debug.Log("Creating new Explosion.");
                    return explosionToSpawn;
                }
            }
        }
        return null;
    }

    public void PutEnemyInHolder(GameObject enemy)
    {
        enemy.transform.SetParent(_enemyHolder);
        enemy.transform.position = _enemyHolder.position;
    }
}
