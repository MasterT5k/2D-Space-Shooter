﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
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
    private GameObject _bossPrefab = null;
    [SerializeField]
    private Transform _bossSpawnPoint = null;

    [Header("Power-Up Settings")]
    [SerializeField]
    private int _minPowerUpSpawnDelay = 3;
    [SerializeField]
    private int _maxPowerUpSpawnDelay = 8;
    [SerializeField]
    [Range(0f, 1f)]
    private float _uncommonChance = 0.31f;
    [SerializeField]
    [Range(0f, 1f)]
    private float _rareChance = 0.1f;
    [SerializeField]
    private GameObject _commonPowerUpPrefab = null;
    [SerializeField]
    private GameObject[] _uncommonPowerUpPrefabs = null;
    [SerializeField]
    private GameObject[] _rarePowerUpPrefabs = null;
    private int _numberOfRaresSpawned;

    private bool _isFinalWave = false;
    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerUps = false;
    private bool _countEnemies = false;
    private bool _isBossActive = false;
    private UIManager _uIManager = null;

    private void Start()
    {
        _numberOfWaves = _waves.Length;

        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.LogError("UIManager is NULL");
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
            if (numberOfEnemies <= 0 && _isBossActive == true)
            {
                _countEnemies = false;
                _stopSpawningPowerUps = true;
                _uIManager.ActivateWinText();
            }
            else if (numberOfEnemies <= 0 && _isFinalWave == false)
            {
                _countEnemies = false;
                _stopSpawningPowerUps = true;
                StopCoroutine("SpawnPowerUpRoutine");
                StartSpawning();
            }
            else if (numberOfEnemies <= 0 && _isFinalWave == true)
            {
                StartCoroutine(SpawnBossRoutine());
                _isFinalWave = false;
            }
        }
    }

    public void StartSpawning()
    {
        _currentWave++;
        if (_currentWave >= _numberOfWaves)
        {
            _currentWave = _numberOfWaves - 1;
            return;
        }
        else if (_currentWave + 1 == _numberOfWaves)
        {
            _isFinalWave = true;
        }

        _stopSpawningEnemies = false;
        _stopSpawningPowerUps = false;
        StartCoroutine(SpawnWaveRoutine(_currentWave));
        StartCoroutine("SpawnPowerUpRoutine");
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
            if (_currentEnemy > wave.enemiesToSpawn.Count - 1)
            {
                _stopSpawningEnemies = true;
                yield break;
            }

            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);
            EnemyType enemyType = wave.enemiesToSpawn[_currentEnemy];
            GameObject enemyObj = enemyType.enemyPrefab;
            Instantiate(enemyObj, spawnLocation, Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(_enemySpawnDelay);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        _numberOfRaresSpawned = 0;
        yield return new WaitForSeconds(_spawnStartDelay);

        while (_stopSpawningPowerUps == false)
        {
            if (_isBossActive == true)
            {
                _numberOfRaresSpawned = -1;
            }
            float randomDelay = Random.Range(_minPowerUpSpawnDelay, _maxPowerUpSpawnDelay);
            float randomX = Random.Range(-_horizontalLimits, _horizontalLimits);
            Vector2 spawnLocation = new Vector2(randomX, _spawnPoint.position.y);

            GameObject powerUp = SelectPowerUp();
            if (powerUp == null)
            {
                yield break;
            }
            Instantiate(powerUp, spawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    IEnumerator SpawnBossRoutine()
    {
        yield return new WaitForSeconds(_enemySpawnDelay);
        Instantiate(_bossPrefab, _bossSpawnPoint.position, Quaternion.identity, _enemyContainer);
        _stopSpawningPowerUps = false;
        StartCoroutine("SpawnPowerUpRoutine");
        _isBossActive = true;
        _countEnemies = true;
    }

    public void StopSpawning()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerUps = true;
    }

    private GameObject SelectPowerUp()
    {
        if (_stopSpawningPowerUps == false)
        {
            float randomChance = Random.value;
            GameObject selectedPowerUp;
            if (randomChance > _uncommonChance)
            {
                selectedPowerUp = _commonPowerUpPrefab;
                return selectedPowerUp;
            }
            else if (randomChance <= _rareChance)
            {
                _numberOfRaresSpawned++;
                int rareLength = _rarePowerUpPrefabs.Length;
                if (_rarePowerUpPrefabs.Length > 1)
                {
                    int randomPowerUp = Random.Range(0, rareLength);
                    selectedPowerUp = _rarePowerUpPrefabs[randomPowerUp];
                }
                else
                {
                    selectedPowerUp = _rarePowerUpPrefabs[0];
                }

                if (_numberOfRaresSpawned >= _waves[_currentWave].numberOfRarePowerUps)
                {
                    selectedPowerUp = _commonPowerUpPrefab;
                }
                return selectedPowerUp;
            }
            else
            {
                int uncommonLength = _uncommonPowerUpPrefabs.Length;
                if (_uncommonPowerUpPrefabs.Length > 1)
                {
                    int randomPowerUp = Random.Range(0, uncommonLength);
                    selectedPowerUp = _uncommonPowerUpPrefabs[randomPowerUp];
                }
                else
                {
                    selectedPowerUp = _uncommonPowerUpPrefabs[0];
                }
                return selectedPowerUp;
            }
        }
        return null;
    }
}
