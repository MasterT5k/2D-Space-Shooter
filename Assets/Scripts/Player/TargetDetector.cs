using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class TargetDetector : MonoBehaviour
{
    private Mine _parent = null;

    void Start()
    {
        _parent = transform.parent.GetComponent<Mine>();
        if (_parent == null)
        {
            Debug.LogError("Parent is Null");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _parent.RecieveTarget(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _parent.LoseTarget(other.transform);
        }
    }
}
