using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyList", menuName = "Scriptable Objects/Spawn Manager/Enemy List")]
public class Enemies : ScriptableObject
{
    public EnemyType[] enemyScriptables;
    private List<EnemyType> _enemyList = new List<EnemyType>();

    public void SetList()
    {
        for (int i = 0; i < enemyScriptables.Length; i++)
        {
            EnemyType enemy = enemyScriptables[i].enemyPrefab.GetComponent<EnemyType>();
            if (enemy != null)
            {
                _enemyList.Add(enemy);
            }
        }
    }

    public GameObject GetEnemyPrefab(int enemyID)
    {
        for (int i = 0; i < enemyScriptables.Length; i++)
        {
            if (_enemyList[i].EnemyClass.GetEnemyID() == enemyID)
            {
                return enemyScriptables[i].enemyPrefab;
            }
        }
        return null;
    }

    public Enemy GetEnemyClass(int enemyID)
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i].EnemyClass.GetEnemyID() == enemyID)
            {
                return _enemyList[i].EnemyClass;
            }
        }
        return null;
    }
}
