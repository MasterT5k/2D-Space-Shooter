using System;
using System.Collections;
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
    private Text _winText = null;
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
    [SerializeField]
    private Slider _masterVolumeSlider = null;
    [SerializeField]
    private Slider _musicVolumeSlider = null;
    [SerializeField]
    private Slider _sFXVolumeSlider = null;
    [SerializeField]
    private CanvasGroup _pausePanelGroup = null;
    [SerializeField]
    private float _fadeMultiplier = 5f;
    private bool _pausePanelOpen = false;
    private bool _panelAnimating = false;

    public static event Action OnPauseGame;
    public static event Action OnQuitGame;
    public static event Action OnLoadMainMenu;
    public static event Action<float> OnSetMasterVolume;
    public static event Action<float> OnSetMusicVolume;
    public static event Action<float> OnSetSFXVolume;
    public static event Func<float> OnGetMasterVolume;
    public static event Func<float> OnGetMusicVolume;
    public static event Func<float> OnGetSFXVolume;

    private void OnEnable()
    {
        SpawnManager.OnUpdateWaves += UpdateWaves;
        SpawnManager.OnNextWave += FlashNextWave;

        Player.OnUpdateAmmo += UpdateAmmo;
        Player.OnUpdateLives += UpdateLivesImage;
        Player.OnUpdateMissiles += UpdateMissile;
        Player.OnUpdateThuster += UpdateThrusterBar;

        BossAI.OnDestroyed += ActivateWinText;

        GameManager.OnUpdateScore += UpdateScore;
        GameManager.OnPauseMenu += AnimatePausePanel;
    }

    private void OnDisable()
    {
        SpawnManager.OnUpdateWaves -= UpdateWaves;
        SpawnManager.OnNextWave -= FlashNextWave;

        Player.OnUpdateAmmo -= UpdateAmmo;
        Player.OnUpdateLives -= UpdateLivesImage;
        Player.OnUpdateMissiles -= UpdateMissile;
        Player.OnUpdateThuster -= UpdateThrusterBar;

        BossAI.OnDestroyed -= ActivateWinText;

        GameManager.OnUpdateScore -= UpdateScore;
        GameManager.OnPauseMenu -= AnimatePausePanel;
    }

    void Start()
    {
        _masterVolumeSlider.value = (float)(OnGetMasterVolume?.Invoke());
        _musicVolumeSlider.value = (float)(OnGetMusicVolume?.Invoke());
        _sFXVolumeSlider.value = (float)(OnGetSFXVolume?.Invoke());
        _pausePanelGroup.alpha = 0;
        _pausePanelGroup.blocksRaycasts = false;
        _pausePanelGroup.interactable = false;

        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _nextWaveText.gameObject.SetActive(false);
        _winText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_pausePanelOpen == false && _panelAnimating == true)
        {
            if (_pausePanelGroup.blocksRaycasts == false)
            {
                _pausePanelGroup.blocksRaycasts = true;
                OnPauseGame?.Invoke();
            }

            _pausePanelGroup.alpha += _fadeMultiplier * Time.unscaledDeltaTime;
            if (_pausePanelGroup.alpha >= 1f)
            {
                _pausePanelGroup.alpha = 1f;
                _pausePanelGroup.interactable = true;
                _panelAnimating = false;
                _pausePanelOpen = true;
            }
        }
        else if (_pausePanelOpen == true && _panelAnimating == true)
        {
            _pausePanelGroup.alpha -= _fadeMultiplier * Time.unscaledDeltaTime;
            if (_pausePanelGroup.alpha <= 0f)
            {
                _pausePanelGroup.alpha = 0f;
                _pausePanelGroup.blocksRaycasts = false;
                _pausePanelGroup.interactable = false;
                _panelAnimating = false;
                _pausePanelOpen = false;
                OnPauseGame?.Invoke();
            }
        }
    }

    public void AnimatePausePanel()
    {
        _panelAnimating = !_panelAnimating;
    }

    public void UpdateLivesImage(int lives)
    {
        _livesImage.sprite = _liveSprites[lives];
        if (lives == 0)
        {
            StartCoroutine(ContinuousFlickerRoutine(_gameOverText));
            _restartText.gameObject.SetActive(true); 
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

    public void ActivateWinText()
    {
        StartCoroutine(ContinuousFlickerRoutine(_winText));
        _restartText.gameObject.SetActive(true);
    }

    public void MainMenuButton()
    {
        OnLoadMainMenu?.Invoke();
    }

    public void QuitGameButton()
    {
        OnQuitGame?.Invoke();
    }

    IEnumerator ContinuousFlickerRoutine(Text textToFlicker)
    {
        while (true)
        {
            textToFlicker.gameObject.SetActive(true);
            yield return new WaitForSeconds(_flickerDelay);
            textToFlicker.gameObject.SetActive(false);
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

    public void UpdateMasterVolume(float volume)
    {
        OnSetMasterVolume?.Invoke(volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        OnSetMusicVolume?.Invoke(volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        OnSetSFXVolume?.Invoke(volume);
    }
}
