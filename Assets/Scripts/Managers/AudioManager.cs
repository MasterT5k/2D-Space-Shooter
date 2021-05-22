using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer _audioMixer = null;
    [SerializeField]
    private AudioSource _sFXSource = null;
    [SerializeField]
    private AudioSource _sFXLoopSource = null;

    private static AudioManager _instance = null;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    Debug.LogError("AudioManager is Null.");
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        Player.OnPlaySFX += PlaySFX;
        Enemy.OnPlaySFX += PlaySFX;
        Explosion.OnPlaySFX += PlaySFX;
        PowerUp.OnPlaySFX += PlaySFX;
        BossAI.OnPlayLoopingSFX += PlayLoopingSFX;
        BossAI.OnStopLoopingSFX += StopLoopingSFX;

        UIManager.OnGetMasterVolume += GetMasterVolume;
        UIManager.OnGetMusicVolume += GetMusicVolume;
        UIManager.OnGetSFXVolume += GetSFXVolume;
        UIManager.OnSetMasterVolume += SetMasterVolume;
        UIManager.OnSetMusicVolume += SetMusicVolume;
        UIManager.OnSetSFXVolume += SetSFXVolume;

        MainMenu.OnGetMasterVolume += GetMasterVolume;
        MainMenu.OnGetMusicVolume += GetMusicVolume;
        MainMenu.OnGetSFXVolume += GetSFXVolume;
        MainMenu.OnSetMasterVolume += SetMasterVolume;
        MainMenu.OnSetMusicVolume += SetMusicVolume;
        MainMenu.OnSetSFXVolume += SetSFXVolume;
    }

    private void OnDisable()
    {
        Player.OnPlaySFX -= PlaySFX;
        Enemy.OnPlaySFX -= PlaySFX;
        Explosion.OnPlaySFX -= PlaySFX;
        PowerUp.OnPlaySFX -= PlaySFX;
        BossAI.OnPlayLoopingSFX -= PlayLoopingSFX;
        BossAI.OnStopLoopingSFX -= StopLoopingSFX;

        UIManager.OnGetMasterVolume -= GetMasterVolume;
        UIManager.OnGetMusicVolume -= GetMusicVolume;
        UIManager.OnGetSFXVolume -= GetSFXVolume;
        UIManager.OnSetMasterVolume -= SetMasterVolume;
        UIManager.OnSetMusicVolume -= SetMusicVolume;
        UIManager.OnSetSFXVolume -= SetSFXVolume;

        MainMenu.OnGetMasterVolume -= GetMasterVolume;
        MainMenu.OnGetMusicVolume -= GetMusicVolume;
        MainMenu.OnGetSFXVolume -= GetSFXVolume;
        MainMenu.OnSetMasterVolume -= SetMasterVolume;
        MainMenu.OnSetMusicVolume -= SetMusicVolume;
        MainMenu.OnSetSFXVolume -= SetSFXVolume;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sFXSource.PlayOneShot(clip);
        }
    }

    private void PlayLoopingSFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sFXLoopSource.clip = clip;
            _sFXLoopSource.Play();
        }
    }

    private void StopLoopingSFX()
    {
        if (_sFXLoopSource.isPlaying == true)
        {
            _sFXLoopSource.Stop();
        }
    }

    private float GetMasterVolume()
    {
        _audioMixer.GetFloat("MasterVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    private float GetMusicVolume()
    {
        _audioMixer.GetFloat("MusicVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    private float GetSFXVolume()
    {
        _audioMixer.GetFloat("SFXVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    private void SetMasterVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

    private void SetSFXVolume(float sliderValue)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }

    private void SetMusicVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
}
