using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstantiator : MonoBehaviour
{
    public GameObject enemyToSpawn;

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

    public void SpawnEnemy()
    {
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
    }
}
