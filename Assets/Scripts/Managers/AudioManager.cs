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

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sFXSource.PlayOneShot(clip);
        }
    }

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sFXLoopSource.clip = clip;
            _sFXLoopSource.Play();
        }
    }

    public void StopLoopingSFX()
    {
        if (_sFXLoopSource.isPlaying == true)
        {
            _sFXLoopSource.Stop();
        }
    }

    public float GetMasterVolume()
    {
        _audioMixer.GetFloat("MasterVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    public float GetMusicVolume()
    {
        _audioMixer.GetFloat("MusicVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    public float GetSFXVolume()
    {
        _audioMixer.GetFloat("SFXVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        return volume;
    }

    public void SetMasterVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
}
