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
    public GameObject explosion;

    public AudioClip newMusic;
    public Transform phase1Tiles;

    SpriteRenderer bg1;
    SpriteRenderer bg2;
    SpriteRenderer bossHeart;
    GameObject boss;
    GameObject boss2;

    Animator anim;
    Animator bgAnim;
    Animator camPointAnim;
    Animator chasteAnim;

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
        camPointAnim = GameObject.Find("CameraFocusPoint").GetComponent<Animator>();
        chasteAnim = GameObject.Find("Chaste_0").GetComponent<Animator>();
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
        boss2 = Instantiate(bossPrefab);
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

    public IEnumerator DeathSequenceCoroutine()
    {
        ewm.isSpawningEnemies = false;
        ply.Freeze();

        if (boss2 == null)
            boss2 = FindObjectOfType<BossPhase2EnemyScript>().gameObject;
        print(boss2 == null);
        cam.target = boss2.transform;

        EnemyScript[] es = FindObjectsOfType<EnemyScript>();
        EnemyProjectile[] ps = FindObjectsOfType<EnemyProjectile>();
        SnotProjectile[] ss = FindObjectsOfType<SnotProjectile>();

        for (int i = 0; i < es.Length; i++)
        {
            if(es[i].gameObject != boss2)
                Destroy(es[i].gameObject);
        }
        for (int i = 0; i < ps.Length; i++)
            Destroy(ps[i].gameObject);
        for (int i = 0; i < ss.Length; i++)
            Destroy(ss[i].gameObject);

        gm.StopMusic();

        Animator b2Anim = boss2.GetComponent<Animator>();
        for (int i = 0; i < 17; i++)
        {
            yield return new WaitForSeconds(0.125f);
            gm.ScreenShake(5);
            Instantiate(explosion, boss2.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3)), Quaternion.identity);
        }

        b2Anim.Play("BossNoSkull");
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(0.11f);
            Instantiate(explosion, boss2.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3)), Quaternion.identity);
        }

        b2Anim.Play("BossHeart");
        yield return new WaitForSeconds(2f);
        Instantiate(explosion, boss2.transform.position, Quaternion.identity);
        Destroy(boss2.gameObject);

        yield return new WaitForSeconds(2);
        anim.Play("BossPhase2End");
    }
    public void MoveCameraPointDown()
    {
        cam.target = camPointAnim.transform;
        camPointAnim.Play("CamPointMoveDown");
        bgAnim.Play("BossBGHidden");
        bg2.color = Color.clear;
    }

    public void DisplayEndDialog()
    {
        StartCoroutine(EndDialog());
    }

    IEnumerator EndDialog()
    {
        yield return gm.WaitForTextCompletion("CutsceneBegin");
        yield return gm.WaitForTextCompletion("Chaste1");
        yield return gm.WaitForTextCompletion("Seb1");
        yield return gm.WaitForTextCompletion("Chaste2");
        yield return gm.WaitForTextCompletion("Seb2");
        yield return gm.WaitForTextCompletion("Slick1");
        yield return gm.WaitForTextCompletion("Chaste3");
        yield return gm.WaitForTextCompletion("Seb3");
        yield return gm.WaitForTextCompletion("Slick2");
        yield return gm.WaitForTextCompletion("Chaste4");
        yield return gm.WaitForTextCompletion("Slick3");
        yield return gm.WaitForTextCompletion("Chaste5");
        yield return new WaitForSeconds(0.25f);
        chasteAnim.Play("ChasteFlipoff");
        yield return new WaitForSeconds(3);
        yield return gm.WaitForTextCompletion("Chaste6");
        chasteAnim.Play("ChasteWalkAway");
        yield return new WaitForSeconds(3);
        yield return gm.WaitForTextCompletion("Slick4");
        yield return gm.WaitForTextCompletion("Seb4");
        yield return gm.WaitForTextCompletion("Slick5");
        yield return gm.WaitForTextCompletion("Seb5");
        yield return new WaitForSeconds(1);
        gm.LevelCompleteSequence();
        Destroy(this.gameObject);
    }
}
