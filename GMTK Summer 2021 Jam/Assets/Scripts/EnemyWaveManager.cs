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

    public EnemyWave[] waves;
    public Transform[] enemyPath;
    public Transform[] spawnPoints;
    public bool[] spawnPointsUseAnimation;
    public GameObject spawner;
    public bool spawnEndlessly;
    public int currentWave;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void StartWaves()
    {
        StartCoroutine(ProcessWavesCoroutine());
    }

    void FixedUpdate()
    {
        if (gm.gameOver)
            StopAllCoroutines();
    }

    IEnumerator ProcessWavesCoroutine()
    {
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
        yield return new WaitForSeconds(2);
        gm.LevelOverSequenece();
    }
}
