using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniShot : MonoBehaviour
{
    [SerializeField]
    private float _maxScale = 20f;
    [SerializeField]
    private float _rateOfExpansion = 30f;
    [SerializeField]
    private float _fadeMultiplier = 20f;

    private SpriteRenderer _renderer = null;
    private bool _isFading = false;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Sprite Renderer is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < _maxScale)
        {
            transform.localScale += Vector3.one * _rateOfExpansion * Time.deltaTime; 
        }
        else if (_isFading == false)
        {
            StartCoroutine(SpriteFadeRoutine());
            _isFading = true;
        }
    }

    IEnumerator SpriteFadeRoutine()
    {
        float opacity = _renderer.color.a;
        while (opacity > 0.1f)
        {
            opacity -= Time.deltaTime * _fadeMultiplier;
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, opacity);
            yield return null;
        }
        Destroy(gameObject);
    }
}
