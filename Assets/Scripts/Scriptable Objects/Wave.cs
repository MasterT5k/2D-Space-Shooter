using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Wave", menuName = "Scriptable Objects/Spawn Manager/Wave")]
public class Wave : ScriptableObject
{
    public List<EnemyType> enemiesToSpawn = new List<EnemyType>();
    public int numberOfRarePowerUps = 2;
}
