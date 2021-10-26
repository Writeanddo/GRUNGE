using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinWaveSpawner : MonoBehaviour
{
    public SpriteRenderer doorHole;
    public GameObject enemyToSpawn;

    GameManager gm;
    AudioSource windAudio;

    bool canCollide = true;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        windAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && canCollide)
        {
            doorHole.color = Color.white;
            //Instantiate(enemyToSpawn, doorHole.transform.position, Quaternion.identity);
            gm.PlaySFX(gm.generalSfx[15]);
            EnemyWaveManager e = FindObjectOfType<EnemyWaveManager>();
            e.StartWaves();
            e.lockedSpawnPoints[0] = false;
            e.lockedSpawnPoints[1] = false;
            e.lockedSpawnPoints[2] = true;
            e.lockedSpawnPoints[3] = false;
            gm.PlayMusic();
            windAudio.Stop();
            canCollide = false;
        }
    }
}
