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
    public GameObject regularExplosion;
    public GameObject dipshitEnemy;

    public AudioClip newMusic;
    public AudioClip carApproach;
    public AudioClip carLeave;
    public Transform phase1Tiles;
    public Animator carAnim;
    public GameObject scythePrefab;
    public Animator cabinAnimator;
    public Animator endlessModePopupAnimator;
    public AudioSource ambienceAudio;
    public AudioSource endMusicAudio;

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
        if (phase2Collision != null)
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
        if (ewm != null)
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
        gm.SetCrosshairVisibility(true);
    }

    public void FadeInBackground()
    {
        StartCoroutine(FadeInSkulls());
        bgAnim.Play("BossBGScroll");
    }

    IEnumerator FadeInSkulls()
    {
        while (bg2.color.a < 1)
        {
            bg2.color = new Color(1, 1, 1, bg2.color.a + 0.025f);
            yield return new WaitForFixedUpdate();
        }
    }

    public void StartScytheSequence()
    {
        StartCoroutine(ScytheSequenceCoroutine());
    }

    public IEnumerator ScytheSequenceCoroutine()
    {
        gm.canPause = false;
        ewm.isSpawningEnemies = false;
        gm.SetCrosshairVisibility(false);
        ply.Freeze();
        FindObjectOfType<HandGrabManager>().DropItem();

        if (boss2 == null)
            boss2 = FindObjectOfType<BossPhase2EnemyScript>().gameObject;
        print(boss2 == null);

        EnemyScript[] es = FindObjectsOfType<EnemyScript>();
        EnemyProjectile[] ps = FindObjectsOfType<EnemyProjectile>();
        SnotProjectile[] ss = FindObjectsOfType<SnotProjectile>();

        for (int i = 0; i < es.Length; i++)
        {
            if (es[i].gameObject != boss2)
                Destroy(es[i].gameObject);
        }
        for (int i = 0; i < ps.Length; i++)
            Destroy(ps[i].gameObject);
        for (int i = 0; i < ss.Length; i++)
            Destroy(ss[i].gameObject);

        // Move player and boss up so they don't get in the way of the car
        ply.transform.position = new Vector2(0, 22);
        boss2.transform.position = new Vector2(22, 22);

        Instantiate(dipshitEnemy, new Vector2(4, 2), Quaternion.identity);

        cam.target = GameObject.Find("CameraFocusPointCar").transform;
        gm.PlaySFX(carApproach);
        carAnim.Play("BossCarApproach");
        yield return new WaitForSeconds(1.5f);
        yield return gm.WaitForTextCompletion("Scythe1");
        gm.PlaySFX(gm.generalSfx[22]);
        carAnim.Play("BossCarTrunk");
        Instantiate(scythePrefab, cam.target.transform.position + new Vector3(5f, 0), Quaternion.identity);
        yield return gm.WaitForTextCompletion("Scythe2");
        gm.PlaySFX(carLeave);
        carAnim.Play("BossCarLeave");
        yield return new WaitForSeconds(1f);
        cam.target = ply.transform;
        ply.canMove = true;
        gm.SetCrosshairVisibility(true);
        boss2.SendMessage("Resume");
        gm.canPause = true;
    }

    public IEnumerator DeathSequenceCoroutine()
    {
        gm.canPause = false;
        gm.SetCrosshairVisibility(false);
        ewm.isSpawningEnemies = false;
        ply.Freeze();

        if (boss2 == null)
            boss2 = FindObjectOfType<BossPhase2EnemyScript>().gameObject;
        print(boss2 == null);
        cam.target = boss2.transform;

        EnemyScript[] es = FindObjectsOfType<EnemyScript>();
        EnemyProjectile[] ps = FindObjectsOfType<EnemyProjectile>();
        SnotProjectile[] ss = FindObjectsOfType<SnotProjectile>();
        GooPickupScript[] gs = FindObjectsOfType<GooPickupScript>();

        for (int i = 0; i < es.Length; i++)
        {
            if (es[i].gameObject != boss2)
                Destroy(es[i].gameObject);
        }
        for (int i = 0; i < ps.Length; i++)
            Destroy(ps[i].gameObject);
        for (int i = 0; i < ss.Length; i++)
            Destroy(ss[i].gameObject);
        for (int i = 0; i < gs.Length; i++)
            Destroy(gs[i].gameObject);

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
        StartCoroutine(FadeInAudio(ambienceAudio));
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
        yield return gm.WaitForTextCompletion("Chaste0");
        yield return new WaitForSeconds(0.5f);
        camPointAnim.Play("CamPointMoveUpThenDown");
        yield return new WaitForSeconds(0.75f);
        Instantiate(regularExplosion, new Vector2(77, 31), Quaternion.identity);
        cabinAnimator.Play("CabinRoofExplode");
        yield return new WaitForSeconds(2.5f);
        yield return gm.WaitForTextCompletion("Chaste0.5");
        yield return gm.WaitForTextCompletion("Slick0");
        yield return EndCabinExplode();
        yield return gm.WaitForTextCompletion("Chaste0.75");
        yield return EndCarApproach();
        yield return gm.WaitForTextCompletion("CutsceneBegin");
        StartCoroutine(FadeInAudio(endMusicAudio));
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
        if(gm.saveVars.furthestUnlockedLevel <= 4)
        {
            endlessModePopupAnimator.Play("EndlessPopup", -1, 0);
            gm.saveVars.furthestUnlockedLevel = 5;
            yield return new WaitForSeconds(5);
        }
        endMusicAudio.Stop();
        gm.LevelCompleteSequence();
        Destroy(this.gameObject);
    }

    IEnumerator EndCabinExplode()
    {
        yield return new WaitForSeconds(0.25f);
        chasteAnim.Play("ChasteShock");
        camPointAnim.Play("CamPointMoveUpThenDownLong");
        gm.PlaySFX(gm.generalSfx[29]);
        StartCoroutine(gm.ScreenShakeCoroutine(2, 3f));
        yield return new WaitForSeconds(1);
        for(int i = 0; i < 8; i++)
        {
            Instantiate(explosion, new Vector2(Random.Range(70, 84f), Random.Range(26, 32f)), Quaternion.identity);
            yield return new WaitForSeconds(1 / 8f);
        }
        
        anim.Play("BossEndHouseExplode");

        for (int i = 0; i < 4; i++)
        {
            Instantiate(explosion, new Vector2(Random.Range(70, 84f), Random.Range(26, 32f)), Quaternion.identity);
            yield return new WaitForSeconds(1 / 8f);
        }

        yield return new WaitForSeconds(2.5f);
        chasteAnim.Play("ChasteIdle");
    }

    IEnumerator EndCarApproach()
    {
        gm.PlaySFX(carApproach);
        yield return new WaitForSeconds(0.5f);
        cabinAnimator.Play("CabinCarApproach");
        yield return new WaitForSeconds(1.5f);

    }

    public void PlayCabinSlimeAnimation()
    {
        ambienceAudio.Stop();
        cabinAnimator.Play("CabinSlimeExplode");
    }

    IEnumerator FadeInAudio(AudioSource a)
    {
        a.Play();
        while(a.volume < 1)
        {
            a.volume += Time.fixedDeltaTime / 2;
            yield return new WaitForFixedUpdate();
        }
        a.volume = 1;
    }

}
