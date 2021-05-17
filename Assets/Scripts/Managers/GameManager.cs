using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private bool _isPaused = false;
    private UIManager _uIManager = null;

    void Start()
    {
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.LogError("UIManager is NULL");
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
            _uIManager.AnimatePausePanel();
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
}
