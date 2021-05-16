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


    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AudioManager is NULL");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
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

    public float GetMasterVolume()
    {
        _audioMixer.GetFloat("MasterVolume", out float volume);
        return volume;
    }

    public float GetSFXVolume()
    {
        _audioMixer.GetFloat("SFXVolume", out float volume);
        return volume;
    }

    public float GetMusicVolume()
    {
        _audioMixer.GetFloat("MusicVolume", out float volume);
        return volume;
    }
}
