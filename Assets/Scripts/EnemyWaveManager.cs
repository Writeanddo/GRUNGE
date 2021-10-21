using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWave
    {
        public List<GameObject> enemiesToSpawn = new List<GameObject>();
        public float delayBetweenSpawning = 2;
        public int pointToSpawnAt;
        public bool useRandomPoint;
        public float timeAfterWave = 10;
    }

    public bool isSpawningEnemies;
    public EnemyWave[] waves;
    public Transform[] enemyPath;
    public Transform[] spawnPoints;
    public Transform[] powerupSpawnPoints;
    public bool[] spawnPointsUseAnimation;
    public GameObject spawner;
    public bool useLoopPath;
    public bool dontRandomizeDirection;
    public bool spawnPowerups = true;
    public bool spawnEndlessly;
    public int currentWave;

    GameObject lastPowerupSpawned;
    public GameObject currentlySpawnedPowerup;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void StartWaves()
    {
        StartCoroutine(ProcessWavesCoroutine());
        if(spawnPowerups)
            StartCoroutine(SpawnPowerupsLoop());
    }

    void FixedUpdate()
    {
        if (gm.gameOver)
            StopAllCoroutines();
    }

    public void ForceStopSpawningEnemies()
    {
        StopAllCoroutines();
    }

    IEnumerator ProcessWavesCoroutine()
    {
        isSpawningEnemies = true;
        bool loopedOnce = false;
        while (spawnEndlessly || !loopedOnce)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                currentWave = i;
                foreach (GameObject g in waves[i].enemiesToSpawn)
                {
                    Transform spawnPoint = null;
                    int pointIndex = 0;
                    if (waves[i].useRandomPoint)
                        pointIndex = Random.Range(0, spawnPoints.Length);
                    else
                        pointIndex = waves[i].pointToSpawnAt;

                    spawnPoint = spawnPoints[pointIndex];
                    if (spawnPointsUseAnimation[pointIndex])
                    {
                        EnemyInstantiator e = Instantiate(spawner, spawnPoint.position, Quaternion.identity).GetComponent<EnemyInstantiator>();
                        e.enemyToSpawn = g;
                    }
                    else
                        Instantiate(g, spawnPoint.position, Quaternion.identity);

                    yield return new WaitForSeconds(waves[i].delayBetweenSpawning);
                }

                yield return new WaitForSeconds(waves[i].timeAfterWave);
            }
            loopedOnce = true;
        }

        EnemyScript[] enemyArray = FindObjectsOfType<EnemyScript>();
        List<EnemyScript> enemyList = enemyArray.OfType<EnemyScript>().ToList();

        while (enemyList.Count > 0)
        {
            enemyList.RemoveAll(item => item == null);
            yield return null;
        }

        print("All enemies killed");
        isSpawningEnemies = false;
        yield return new WaitForSeconds(2);
        gm.LevelOverSequenece();
    }

    IEnumerator SpawnPowerupsLoop()
    {
        yield return new WaitForSeconds(7);
        while (isSpawningEnemies)
        {
            float rand = Random.Range(0, 1f);
            print("Powerup chance was " + rand);
            if (rand > 0.75f && currentlySpawnedPowerup == null)
            {
                SpawnPowerup();
                yield return new WaitForSeconds(9);
            }

            yield return new WaitForSeconds(9);
        }
    }

    void SpawnPowerup()
    {
        if (powerupSpawnPoints.Length == 0)
            return;

        gm.PlaySFX(gm.generalSfx[14]);
        int rand = Random.Range(0, gm.powerups.Length);
        int randPosition = Random.Range(0, powerupSpawnPoints.Length);
        EnemyInstantiator e = Instantiate(spawner, powerupSpawnPoints[randPosition].position, Quaternion.identity).GetComponent<EnemyInstantiator>();

        while (lastPowerupSpawned != null && gm.powerups[rand] != lastPowerupSpawned)
            rand = Random.Range(0, gm.powerups.Length);

        e.enemyToSpawn = gm.powerups[rand];
    }
}
