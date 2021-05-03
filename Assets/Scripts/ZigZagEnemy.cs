using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagEnemy : Enemy
{
    [SerializeField]
    private float _sideMovementSpeed = 4f;

    protected override void CalulateMovement()
    {
        base.CalulateMovement();

        //Side to Side Movement Code
    }
}
