﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCutscene : MonoBehaviour
{
    public GameObject wallToDisable;
    public GameObject smallExplosion;
    public GameObject bigExplosion;
    public AudioClip secondExplosionSound;
    public GameObject[] initialEnemies;

    EnemyWaveManager ewm;

    GameManager gm;

    bool hasExploded;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ewm = FindObjectOfType<EnemyWaveManager>();
    }

    public void ExplodeWall()
    {
        StartCoroutine(ExplodeBathroomCoroutine());
    }

    private void Update()
    {
        if(ewm.currentWave == 3 && !hasExploded)
        {
            hasExploded = true;
            StartCoroutine(ExplodeBedroomCoroutine());
        }
    }

    IEnumerator ExplodeBathroomCoroutine()
    {
        hasExploded = true;
        Instantiate(smallExplosion, new Vector3(-14, 5, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-8, 6, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-12, 9, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-15, 7, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Instantiate(bigExplosion);
        gm.PlaySFX(secondExplosionSound);

        Instantiate(initialEnemies[0], new Vector2(-15, 7), Quaternion.identity);
        Instantiate(initialEnemies[1], new Vector2(-12, 8), Quaternion.identity);
        Instantiate(initialEnemies[2], new Vector2(-20, 9), Quaternion.identity);
        Instantiate(initialEnemies[3], new Vector2(-16, 9), Quaternion.identity);

        wallToDisable.SetActive(false);

        ewm.StartWaves();
        
        Destroy(this.gameObject);
    }

    IEnumerator ExplodeBedroomCoroutine()
    {
        hasExploded = true;
        Instantiate(smallExplosion, new Vector3(-11, -2, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-16, -3, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-8, -4, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(smallExplosion, new Vector3(-12, -5, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Instantiate(bigExplosion, new Vector3(-10.75f, -2.25f, 0), Quaternion.identity);
        gm.PlaySFX(secondExplosionSound);

        Instantiate(initialEnemies[0], new Vector2(-14, -6), Quaternion.identity);
        Instantiate(initialEnemies[1], new Vector2(-15, -7), Quaternion.identity);
        Instantiate(initialEnemies[2], new Vector2(-15, -5), Quaternion.identity);
        Instantiate(initialEnemies[3], new Vector2(-14, -8), Quaternion.identity);

        wallToDisable.SetActive(false);

        hasExploded = true;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gm.PlayMusic();
            GetComponent<Animator>().Play("DoorOpen");
        }
    }
}