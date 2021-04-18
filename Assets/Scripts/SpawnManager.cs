using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab = null;
    [SerializeField]
    private Transform _spawnPoint = null;
    [SerializeField]
    private Transform _enemyContainer = null;
    [SerializeField]
    private float _horizontalLimits = 8f;
    [SerializeField]
    private float _spawnStartDelay = 3f;
    [SerializeField]
    private float _enemySpawnDelay = 5f;
    [SerializeField]
    private int _minPowerUpSpawnDelay = 3, _maxPowerUpSpawnDelay = 8;
    [SerializeField]
    private GameObject[] _powerUpPrefabs = null;

    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_spawnStartDelay);

        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);

            Instantiate(_enemyPrefab, spawnLocation, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_enemySpawnDelay);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(_spawnStartDelay);

        while (_stopSpawning == false)
        {
            float randomDelay = Random.Range(_minPowerUpSpawnDelay, _maxPowerUpSpawnDelay);
            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);

            Instantiate(_powerUpPrefabs[Random.Range(0, _powerUpPrefabs.Length)], spawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}
