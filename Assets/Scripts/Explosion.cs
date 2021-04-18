using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float _destroyDelay = 3f;

    void Start()
    {
        Destroy(gameObject, _destroyDelay);
    }
}
