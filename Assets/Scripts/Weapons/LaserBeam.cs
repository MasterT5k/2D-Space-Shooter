using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class LaserBeam : MonoBehaviour
{
    [SerializeField]
    private int _damage = 1;
    private bool _isEnemyBeam = false;

    public static event Action<int> OnDamagePlayer;

    public void AssignEnemyBeam()
    {
        _isEnemyBeam = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyBeam == true)
        {
            OnDamagePlayer?.Invoke(-_damage);
        }
    }
}
