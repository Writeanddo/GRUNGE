using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase2Manager : MonoBehaviour
{
    [System.Serializable]
    public class BossPath
    {
        public Transform[] nodes;
    }

    public BossPath[] paths;
    public GameObject bossPrefab;
    public GameObject phase2Collision;

    public AudioClip newMusic;
    public Transform phase1Tiles;

    SpriteRenderer bg1;
    SpriteRenderer bg2;
    SpriteRenderer bossHeart;
    GameObject boss;

    Animator anim;
    Animator bgAnim;

    GameManager gm;
    PlayerController ply;
    CameraController cam;

    EnemyWaveManager ewm;

    // Start is called before the first frame update
    void Start()
    {
        if(phase2Collision != null)
            phase2Collision.transform.position = new Vector2(300, 0);

        anim = GetComponent<Animator>();
        bg1 = GameObject.Find("BackgroundWavy").GetComponent<SpriteRenderer>();
        bg2 = GameObject.Find("BackgroundSkulls").GetComponent<SpriteRenderer>();
        bossHeart = GameObject.Find("BossHeart").GetComponent<SpriteRenderer>();
        boss = GameObject.Find("Boss");
        bossHeart.color = Color.clear;
        cam = FindObjectOfType<CameraController>();
        bgAnim = bg1.GetComponentInParent<Animator>();
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        ewm = FindObjectOfType<EnemyWaveManager>();
    }

    public IEnumerator PlayTransformationCutscene()
    {
        gm.music = newMusic;
        gm.PlayMusic();
        anim.Play("BossPhase1End", -1, 0);
        yield return null;
    }

    public void SetNodePath(int index)
    {
        if(ewm != null)
            ewm.enemyPath = paths[index].nodes;
    }

    public void SetupArena()
    {
        bgAnim.Play("BossBGHalfScroll");
        SetNodePath(0);
        gm.canPause = true;
        GameObject g = Instantiate(bossPrefab);
        cam.secondTarget = null;
        cam.target = ply.transform;
        cam.overridePosition = false;

        phase1Tiles.transform.position = new Vector2(300, 0);
        phase2Collision.transform.position = Vector2.zero;

        Destroy(boss);
        bg1.sortingLayerName = "Background";
        bg2.sortingLayerName = "Background";
        bossHeart.color = Color.clear;
        ply.transform.position = new Vector2(12, 10);
        ply.canMove = true;
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
