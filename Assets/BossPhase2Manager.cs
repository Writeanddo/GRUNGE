using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase2Manager : MonoBehaviour
{
    public AudioClip newMusic;
    SpriteRenderer bg1;
    SpriteRenderer bg2;
    SpriteRenderer bossHeart;

    Animator anim;
    Animator bgAnim;

    GameManager gm;
    PlayerController ply;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        bg1 = GameObject.Find("BackgroundWavy").GetComponent<SpriteRenderer>();
        bg2 = GameObject.Find("BackgroundSkulls").GetComponent<SpriteRenderer>();
        bossHeart = GameObject.Find("BossHeart").GetComponent<SpriteRenderer>();
        bossHeart.color = Color.clear;
        bgAnim = bg1.GetComponentInParent<Animator>();
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
    }

    public IEnumerator PlayTransformationCutscene()
    {
        gm.music = newMusic;
        gm.PlayMusic();
        anim.Play("BossPhase1End", -1, 0);
        yield return null;
    }

    public void SetupArena()
    {
        bossHeart.color = Color.clear;
        ply.transform.position = new Vector2(12, 10);
    }

    public void FadeInBackground()
    {
        StartCoroutine(FadeInSkulls());
        bgAnim.Play("BossBGScroll");
    }

    IEnumerator FadeInSkulls()
    {
        while(bg2.color.a < 1)
        {
            bg2.color = new Color(1, 1, 1, bg2.color.a + 0.025f);
            yield return new WaitForFixedUpdate();
        }
    }
}
