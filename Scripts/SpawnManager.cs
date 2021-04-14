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
    private GameObject[] _powerUpPrefabs = null;

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-8f, 8f);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);

            Instantiate(_enemyPrefab, spawnLocation, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_stopSpawning == false)
        {
            float randomDelay = Random.Range(3, 8);
            float randomX = Random.Range(-8f, 8f);
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
