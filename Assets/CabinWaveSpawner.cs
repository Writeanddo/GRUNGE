using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinWaveSpawner : MonoBehaviour
{
    public SpriteRenderer doorHole;
    public GameObject enemyToSpawn;

    GameManager gm;
    AudioSource inAudio;
    AudioSource outAudio;

    bool canCollide = true;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        inAudio = GameObject.Find("GameMusicTrackInstruments").GetComponent<AudioSource>();
        outAudio = GameObject.Find("GameMusicTrackDrums").GetComponent<AudioSource>();
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
            Instantiate(enemyToSpawn, doorHole.transform.position, Quaternion.identity);
            gm.PlaySFX(gm.generalSfx[15]);
            FindObjectOfType<EnemyWaveManager>().StartWaves();
            StartCoroutine(gm.FadeMusic());
            canCollide = false;
        }
    }
}
