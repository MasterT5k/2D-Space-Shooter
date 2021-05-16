using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float _destroyDelay = 3f;
    [SerializeField]
    private AudioClip _explosionClip = null;

    void Start()
    {
        AudioManager.Instance.PlaySFX(_explosionClip);
        Destroy(gameObject, _destroyDelay);
    }
}
