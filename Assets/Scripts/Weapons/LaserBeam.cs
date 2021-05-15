using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class LaserBeam : MonoBehaviour
{
    private bool _isEnemyBeam = false;

    public void AssignEnemyBeam()
    {
        _isEnemyBeam = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyBeam == true)
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.ChangeLives();
            }
        }
    }
}
