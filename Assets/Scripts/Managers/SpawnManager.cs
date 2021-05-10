using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //[SerializeField]
    //private GameObject[] _enemyPrefabs = null;
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
    private Wave[] _waves = null;
    private int _currentWave = -1;
    private int _numberOfWaves;
    private int _currentEnemy = -1;

    [SerializeField]
    private int _minPowerUpSpawnDelay = 3, _maxPowerUpSpawnDelay = 8;
    [SerializeField]
    private GameObject[] _powerUpPrefabs = null;

    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerUps = false;
    private bool _countEnemies = false;
    private UIManager _uIManager = null;

    private void Start()
    {
        _numberOfWaves = _waves.Length;

        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.Log("UIManager is NULL");
        }
        else
        {
            _uIManager.UpdateWaves(_currentWave, _numberOfWaves); 
        }
    }

    private void Update()
    {
        if (_countEnemies == true)
        {
            int numberOfEnemies = _enemyContainer.childCount;
            if (numberOfEnemies <= 0)
            {
                _countEnemies = false;
                _stopSpawningPowerUps = true;
                StartSpawning();
            }
        }
    }

    public void StartSpawning()
    {
        _currentWave++;
        if (_currentWave >= _numberOfWaves)
        {
            return;
        }

        _stopSpawningEnemies = false;
        _stopSpawningPowerUps = false;
        StartCoroutine(SpawnWaveRoutine(_currentWave));
        StartCoroutine(SpawnPowerUpRoutine());
        _uIManager.UpdateWaves(_currentWave + 1, _numberOfWaves);
        _uIManager.FlashNextWave();
    }

    IEnumerator SpawnWaveRoutine(int waveNumber)
    {
        Wave wave = _waves[waveNumber];
        _currentEnemy = -1;
        yield return new WaitForSeconds(_spawnStartDelay);
        _countEnemies = true;
        while (_stopSpawningEnemies == false)
        {
            _currentEnemy++;
            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);
            EnemyType enemyType = wave.enemiesToSpawn[_currentEnemy];
            GameObject enemyObj = enemyType.enemyPrefab;
            Instantiate(enemyObj, spawnLocation, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_enemySpawnDelay);
            if (_currentEnemy + 1 >= wave.enemiesToSpawn.Count)
            {
                _stopSpawningEnemies = true;
            }
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(_spawnStartDelay);

        while (_stopSpawningPowerUps == false)
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
        _stopSpawningEnemies = true;
        _stopSpawningPowerUps = true;
    }
}
