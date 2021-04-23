using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float _magnitude = 0.2f;
    [SerializeField]
    private float _duration = 0.15f;
    private bool _isShakeRunning = false;

    public void ShakeCamera()
    {
        if (_isShakeRunning == false)
        {
            StartCoroutine(Shake(_duration, _magnitude));
        }
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        _isShakeRunning = true;

        Vector3 startPosition = transform.localPosition;

        float timeLeft = duration;

        while (timeLeft > 0)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Vector3 nextPosition = new Vector3(x, y);
            transform.localPosition = nextPosition;
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPosition;
        _isShakeRunning = false;
    }
}
