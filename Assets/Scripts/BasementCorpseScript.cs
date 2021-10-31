using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementCorpseScript : MonoBehaviour
{
    Animator anim;
    EnemyWaveManager ewm;
    GameManager gm;
    AudioSource audio;
    bool collided;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        ewm = FindObjectOfType<EnemyWaveManager>();
    }

    public void StartWaves()
    {
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        yield return new WaitForSeconds(1.5f);
        audio.Stop();
        gm.PlayMusic();
        ewm.StartWaves();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !collided)
        {
            collided = true;
            gm.PlaySFX(gm.generalSfx[27]);
            anim.Play("CorpseExplode");
        }
    }
}
