using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Slider _masterVolumeSlider = null;
    [SerializeField]
    private Slider _musicVolumeSlider = null;
    [SerializeField]
    private Slider _sFXVolumeSlider = null;

    private Animator _anim = null;

    public static event Action<float> OnSetMasterVolume;
    public static event Action<float> OnSetMusicVolume;
    public static event Action<float> OnSetSFXVolume;
    public static event Func<float> OnGetMasterVolume;
    public static event Func<float> OnGetMusicVolume;
    public static event Func<float> OnGetSFXVolume;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _masterVolumeSlider.value = (float)(OnGetMasterVolume?.Invoke());
        _musicVolumeSlider.value = (float)(OnGetMusicVolume?.Invoke());
        _sFXVolumeSlider.value = (float)(OnGetSFXVolume?.Invoke());
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

    public void ShowOptions()
    {
        _anim.SetBool("ShowOptions", true);
    }

    public void HideOptions()
    {
        _anim.SetBool("ShowOptions", false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
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
