using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    // Find the farthest enemy from the player
    public GameObject GetFarthestEnemy(Vector3 playerPosition)
    {
        GameObject farthestEnemy = null;
        float maxDistance = 0f;

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null) // Check if enemy is still active
            {
                float distance = Vector3.Distance(playerPosition, enemy.transform.position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestEnemy = enemy;
                }
            }
        }

        return farthestEnemy;
    }
}
