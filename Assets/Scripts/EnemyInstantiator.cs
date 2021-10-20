using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstantiator : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public bool spawningPowerup;

    float yOffset = -0.75f;

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

    public void SpawnEnemy()
    {
        GameObject g = Instantiate(enemyToSpawn, transform.position + Vector3.up*yOffset, Quaternion.identity);
        if (spawningPowerup)
            FindObjectOfType<EnemyWaveManager>().currentlySpawnedPowerup = g;
    }
}
