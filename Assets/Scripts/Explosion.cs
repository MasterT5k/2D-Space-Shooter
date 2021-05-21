using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Explosion : MonoBehaviour
{
    [SerializeField]
    private int _explosionID = -1;
    [SerializeField]
    private float _destroyDelay = 3f;
    [SerializeField]
    private AudioClip _explosionClip = null;
    private Animator _anim = null;
    private bool _isReused = false;

    private void OnEnable()
    {
        if (_isReused == true)
        {
            AudioManager.Instance.PlaySFX(_explosionClip);
            StartCoroutine(DeactivateRoutine());
        }
    }

    private void OnDisable()
    {
        if (_isReused == true)
        {
            transform.position = transform.parent.position;
            _anim.SetTrigger("Explode");
        }
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
        AudioManager.Instance.PlaySFX(_explosionClip);
        StartCoroutine(DeactivateRoutine());
        _isReused = true;
    }

    IEnumerator DeactivateRoutine()
    {
        _anim.SetTrigger("Explode");
        yield return new WaitForSeconds(_destroyDelay);
        gameObject.SetActive(false);
    }

    public int GetExplosionID()
    {
        return _explosionID;
    }
}
