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
        public int wavePower = 0; // Used in endless mode to make sure we don't spawn too many tough enemies
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
    public bool[] lockedSpawnPoints;
    public bool[] spawnPointsUseAnimation;
    public GameObject spawner;
    public bool useLoopPath;
    public bool dontRandomizeDirection;
    public bool spawnPowerups = true;
    public bool spawnEndlessly;
    public int currentWave;
    public int numWavesCleared;

    public float endlessSpawnTime = 2.5f;

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
        if (gm.playingEndlessMode)
        {
            waves = gm.endlessModeWaves;
            spawnEndlessly = true;
        }

        StartCoroutine(ProcessWavesCoroutine());
        if (spawnPowerups)
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

    IEnumerator EndlessDifficultyIncreaser()
    {
        yield return new WaitForSeconds(10);
        endlessSpawnTime = Mathf.Clamp(endlessSpawnTime * 0.93f, 1.25f, 4);
        StartCoroutine(EndlessDifficultyIncreaser());
    }

    IEnumerator ProcessWavesCoroutine()
    {
        if (gm.playingEndlessMode)
            StartCoroutine(EndlessDifficultyIncreaser());

        isSpawningEnemies = true;
        int accumulatedPower = 0;
        bool loopedOnce = false;
        while (spawnEndlessly || !loopedOnce)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                // Randomly choose wave if in endless mode
                if (gm.playingEndlessMode)
                {
                    accumulatedPower++;
                    int lastWave = currentWave;
                    while(currentWave == lastWave || waves[currentWave].wavePower > accumulatedPower)
                        currentWave = Random.Range(0, waves.Length);

                    accumulatedPower -= waves[currentWave].wavePower;
                }
                else
                    currentWave = i;

                foreach (GameObject g in waves[currentWave].enemiesToSpawn)
                {
                    Transform spawnPoint = null;
                    int pointIndex = 0;
                    if (waves[currentWave].useRandomPoint)
                    {
                        pointIndex = Random.Range(0, spawnPoints.Length);
                        while (lockedSpawnPoints[pointIndex])
                            pointIndex = Random.Range(0, spawnPoints.Length);
                    }
                    else
                        pointIndex = waves[currentWave].pointToSpawnAt;

                    spawnPoint = spawnPoints[pointIndex];
                    if (spawnPointsUseAnimation[pointIndex])
                    {
                        if (gm.enemiesInLevel.Count < 15)
                        {
                            EnemyInstantiator e = Instantiate(spawner, spawnPoint.position, Quaternion.identity).GetComponent<EnemyInstantiator>();
                            e.enemyToSpawn = g;
                        }
                    }
                    else
                        Instantiate(g, spawnPoint.position, Quaternion.identity);

                    float delayBetweenEnemies = waves[currentWave].delayBetweenSpawning;
                    if (gm.playingEndlessMode)
                        delayBetweenEnemies = endlessSpawnTime;
                    yield return new WaitForSeconds(delayBetweenEnemies);
                }

                float delay = waves[currentWave].timeAfterWave;
                if (gm.playingEndlessMode)
                {
                    delay = endlessSpawnTime;
                }
                yield return new WaitForSeconds(delay);
                numWavesCleared++;
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
