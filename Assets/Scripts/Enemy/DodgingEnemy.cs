using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingEnemy : Enemy
{
    [Header("Dodge Settings")]
    [SerializeField]
    private float _dodgeSpeed = 2f;
    private bool _isDodging = false;
    private bool _isMovingRight = false;
    private List<GameObject> _laserDodgeList = new List<GameObject>();

    protected override void CalulateMovement()
    {
        base.CalulateMovement();

        if (_isDead == false)
        {
            DodgeMovement();
        }
    }

    void DodgeMovement()
    {
        if (_laserDodgeList.Count == 0 && _isDodging == true)
        {
            _isDodging = false;
        }
        else if (_isDodging == true)
        {

            if (_isMovingRight == false)
            {
                transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
            }
        }
    }

    public void ReceiveLaserToDodge(GameObject laser)
    {
        _laserDodgeList.Add(laser);

        if (_laserDodgeList.Count == 1)
        {
            _isDodging = true;
            FigureOutDirection(laser);
            return;
        }
    }

    public void CheckDodgedLaser(GameObject laser)
    {
        if (_laserDodgeList.Contains(laser))
        {
            _laserDodgeList.Remove(laser);
            
            if (_laserDodgeList.Count > 0)
            {
                if (_laserDodgeList[0] == null)
                {
                    Debug.LogError("Laser List Element is NULL");
                    return;
                }

                FigureOutDirection(_laserDodgeList[0]);
            }
            else
            {
                _isDodging = false; 
            }
        }
    }

    void FigureOutDirection(GameObject laser)
    {
        float enemyXAxis = transform.position.x;
        float laserXAxis = laser.transform.position.x;

        //Laser is on the Left of the screen from Enemy.
        if (laserXAxis <= enemyXAxis)
        {
            //Enemy needs to move Right to avoid.
            _isMovingRight = true;
        }
        //Laser is on the Right of the screen from Enemy.
        else if (laserXAxis > enemyXAxis)
        {
            //Enemy needs to move Left to avoid.
            _isMovingRight = false;
        }
    }
}
