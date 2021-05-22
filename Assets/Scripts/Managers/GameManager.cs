using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private int _score;
    private bool _isGameOver = false;
    private bool _isPaused = false;
    private GameObject _playerObj = null;
    private Player _playerScript = null;

    public static event Action OnPauseMenu;
    public static event Action<int> OnUpdateScore;
    public static event Func<GameObject> OnGetPlayerObj;

    void OnEnable()
    {
        Player.OnDeath += GameOver;
        Enemy.OnScored += ChangeScore;
        Enemy.OnGetPlayerScript += GivePlayerScript;
        Mine.OnScored += ChangeScore;
        UIManager.OnPauseGame += PauseGame;
        UIManager.OnLoadMainMenu += LoadMainMenu;
        UIManager.OnQuitGame += QuitGame;
        BossAI.OnGetPlayerObject += GivePlayerObj;
        BossAI.OnGetPlayerScript += GivePlayerScript;
        BossAI.OnDestroyed += GameOver;
        BossAI.OnScored += ChangeScore;
        PowerUp.OnGetPlayerObj += GivePlayerObj;
        PowerUp.OnGetPlayerScript += GivePlayerScript;
        BeamEmitter.OnScored += ChangeScore;
    }

    void OnDisable()
    {
        Player.OnDeath -= GameOver;
        Enemy.OnScored -= ChangeScore;
        Enemy.OnGetPlayerScript -= GivePlayerScript;
        Mine.OnScored -= ChangeScore;
        UIManager.OnPauseGame -= PauseGame;
        UIManager.OnLoadMainMenu -= LoadMainMenu;
        UIManager.OnQuitGame -= QuitGame;
        BossAI.OnGetPlayerObject -= GivePlayerObj;
        BossAI.OnGetPlayerScript -= GivePlayerScript;
        BossAI.OnDestroyed -= GameOver;
        BossAI.OnScored -= ChangeScore;
        PowerUp.OnGetPlayerObj -= GivePlayerObj;
        PowerUp.OnGetPlayerScript -= GivePlayerScript;
        BeamEmitter.OnScored -= ChangeScore;
    }

    void Start()
    {
        OnUpdateScore?.Invoke(_score);
        _playerObj = OnGetPlayerObj?.Invoke();
        if (_playerObj != null)
        {
            _playerScript = _playerObj.GetComponent<Player>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseMenu?.Invoke();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;

        if (_isPaused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void LoadMainMenu()
    {
        PauseGame();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    private GameObject GivePlayerObj()
    {
        if (_playerObj != null)
        {
            return _playerObj;
        }
        return null;
    }

    private Player GivePlayerScript()
    {
        if (_playerScript != null)
        {
            return _playerScript;
        }
        return null;
    }

    private void ChangeScore(int amount)
    {
        _score += amount;
        OnUpdateScore?.Invoke(_score);
    }
}
