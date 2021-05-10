﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText = null;
    [SerializeField]
    private Image _livesImage = null;
    [SerializeField]
    private Sprite[] _liveSprites = null;
    [SerializeField]
    private Slider _thrusterSlider;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Text _missileText;
    [SerializeField]
    private Text _gameOverText = null;
    [SerializeField]
    private Text _restartText = null;
    [SerializeField]
    private Text _waveText = null;
    [SerializeField]
    private Text _nextWaveText = null;
    [SerializeField]
    private float _flickerDelay = 0.5f;
    [SerializeField]
    private float _flickerDuration = 2f;

    private GameManager _gameManager = null;

    void Start()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL");
        }

        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _nextWaveText.gameObject.SetActive(false);
    }

    public void UpdateLivesImage(int lives)
    {
        _livesImage.sprite = _liveSprites[lives];
        if (lives == 0)
        {
            StartCoroutine(GameOverFlickerRoutine());
            _restartText.gameObject.SetActive(true);
            _gameManager.GameOver();
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score:" + score;
    }

    public void UpdateAmmo(int ammoRemaining, int maxAmmo)
    {
        _ammoText.text = "Ammo:" + ammoRemaining + "/" + maxAmmo;
    }

    public void UpdateMissile(int missilesRemaining, int maxMissiles)
    {
        _missileText.text = "Missiles:" + missilesRemaining + "/" + maxMissiles;
    }

    public void UpdateThrusterBar(float elapsedTime, float burnLength)
    {
        if (_thrusterSlider.maxValue != burnLength)
        {
            _thrusterSlider.maxValue = burnLength;
        }
        _thrusterSlider.value = burnLength -= elapsedTime;
    }

    public void UpdateWaves(int currentWave , int maxWaves)
    {
        if (currentWave < 0)
        {
            currentWave = 0;
        }
        _waveText.text = "Waves:" + currentWave + "/" + maxWaves;
    }

    public void FlashNextWave()
    {
        StartCoroutine(NextWaveFlickerRoutine());

    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(_flickerDelay);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(_flickerDelay);
        }
    }



    IEnumerator NextWaveFlickerRoutine()
    {
        int counter = 0;
        while (counter < _flickerDuration)
        {
            _nextWaveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(_flickerDelay);
            _nextWaveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(_flickerDelay);
            counter++;
        }
    }
}
