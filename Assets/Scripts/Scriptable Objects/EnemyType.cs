using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Enemy", menuName = "Scriptable Objects/Spawn Manager/Enemy")]
public class EnemyType : ScriptableObject
{
    private Enemy _enemyClass;
    public GameObject enemyPrefab;
    public Enemy EnemyClass
    {
        get
        {
            if (_enemyClass == null)
                _enemyClass = enemyPrefab.GetComponent<Enemy>();

            return _enemyClass;
        }
    }
}
