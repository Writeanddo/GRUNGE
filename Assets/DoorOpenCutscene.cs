using System.Collections;
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

    IEnumerator ExplodeBathroomCoroutine()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            GetComponent<Animator>().Play("DoorOpen");
    }
}
