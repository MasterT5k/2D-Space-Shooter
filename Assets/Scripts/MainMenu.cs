using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class MainMenu : MonoBehaviour
{
    Animator _anim = null;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowControls()
    {
        _anim.SetBool("ShowControls", true);
    }

    public void HideControls()
    {
        _anim.SetBool("ShowControls", false);
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
