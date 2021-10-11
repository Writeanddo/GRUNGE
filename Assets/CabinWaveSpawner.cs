using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinWaveSpawner : MonoBehaviour
{
    public SpriteRenderer doorHole;
    public GameObject enemyToSpawn;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            doorHole.color = Color.white;
            Instantiate(enemyToSpawn, doorHole.transform.position, Quaternion.identity);
            gm.PlaySFX(gm.generalSfx[15]);
            FindObjectOfType<EnemyWaveManager>().StartWaves();
            Destroy(this.gameObject);
        }
    }
}
