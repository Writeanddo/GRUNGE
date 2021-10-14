using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstantiator : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public bool spawningPowerup;

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

    public void SpawnEnemy()
    {
        GameObject g = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        if (spawningPowerup)
            FindObjectOfType<EnemyWaveManager>().currentlySpawnedPowerup = g;
    }
}
