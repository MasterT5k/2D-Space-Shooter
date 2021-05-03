using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearFireEnemy : Enemy
{
    [SerializeField]
    private Transform _rearLaserSpawn = null;

    protected override void Start()
    {
        base.Start();

        DeterminePlayerPosition();
    }

    protected override void Update()
    {
        DeterminePlayerPosition();

        base.Update();
    }

    protected override void FireLaser()
    {
        //If Player is infront of the Enemy Fire Normally.
        base.FireLaser();
        //Else Player is behind the Enemy Fire Backwards.
        //Spawn Laser Behind the Enemy.
        PlayClip(_laserClip);
        //End else
    }

    void DeterminePlayerPosition()
    {
        //Determine where the Player is.
    }
}
