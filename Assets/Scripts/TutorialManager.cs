using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject spawner;
    public GameObject[] itemsToSpawn;
    public Transform[] spawnPoints;
    public GameObject stairsBlocker;
    public Animator camAnimator;

    GameManager gm;
    PlayerController ply;

    bool exitedDoor;

    // Start is called before the first frame update
    void Start()
    {
        ply = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnObject(Vector3 position, GameObject g)
    {
        EnemyInstantiator e = Instantiate(spawner, position, Quaternion.identity).GetComponent<EnemyInstantiator>();
        e.enemyToSpawn = g;
    }

    public IEnumerator IntroDialog()
    {
        yield return gm.WaitForTextCompletion("TutorialIntro");
        ply.canMove = true;

        yield return WaitForMovement();
        yield return new WaitForSeconds(2.5f);
        ply.Freeze();
        yield return gm.WaitForTextCompletion("WalkingFollowup");
        yield return gm.WaitForTextCompletion("WalkingFollowup2");
        yield return gm.WaitForTextCompletion("WalkingFollowup3");
        SpawnObject(spawnPoints[0].position, itemsToSpawn[0]);
        yield return gm.WaitForTextCompletion("GrabEnemy");
        ply.canMove = true;
        ply.canLaunchHand = true;
        while (ply.heldObject == null)
            yield return null;

        Transform t = ply.heldObject;
        ply.Freeze();
        yield return gm.WaitForTextCompletion("GrabEnemyFollowup");
        ply.ForceRightClickRelease();
        ply.canMove = true;

        while (t != null)
            yield return null;

        yield return new WaitForSeconds(0.5f);
        ply.Freeze();
        yield return gm.WaitForTextCompletion("ThrowEnemyFollowup");

        
        SpawnObject(spawnPoints[0].position, itemsToSpawn[0]);
        SpawnObject(spawnPoints[1].position, itemsToSpawn[2]);
        SpawnObject(spawnPoints[2].position, itemsToSpawn[1]);
        SpawnObject(spawnPoints[3].position, itemsToSpawn[2]);

        yield return gm.WaitForTextCompletion("ThrowEnemyFollowup2");
        ply.canMove = true;

        Grabbable[] g = FindObjectsOfType<Grabbable>();
        List<Grabbable> e1 = new List<Grabbable>();
        List<Grabbable> e2 = new List<Grabbable>(); //non-grabbable enemies
        Grabbable shovel = null;

        for (int i = 0; i < g.Length; i++)
        {
            if (g[i].name == "TrainingDummy2(Clone)")
                e2.Add(g[i]);
            else if (g[i].name == "TrainingDummy1(Clone)")
                e1.Add(g[i]);
            else
                shovel = g[i];
        }

        bool enemiesDefeated = false;
        bool stuck = false;
        while (!enemiesDefeated)
        {
            e1.RemoveAll(x => x == null);
            e2.RemoveAll(x => x == null);

            if (e2.Count == 0 && e1.Count == 0)
                enemiesDefeated = true;
            else if (e1.Count == 0 && shovel == null && e2.Count > 0)
            {
                stuck = true;
                enemiesDefeated = true;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        ply.Freeze();
        if (stuck)
            yield return gm.WaitForTextCompletion("ThrowEnemyFollowupSoftlock");
        else
            yield return gm.WaitForTextCompletion("ThrowEnemyFollowupNonSoftlock");

        foreach(Grabbable gr in e2)
            Destroy(gr.gameObject);
        if(shovel != null)
            Destroy(shovel.gameObject);

        yield return gm.WaitForTextCompletion("GunIntro");
        
        ply.stats.goo = 60;
        yield return gm.WaitForTextCompletion("GunIntro2");
        SpawnObject(spawnPoints[0].position, itemsToSpawn[3]);
        SpawnObject(spawnPoints[2].position, itemsToSpawn[3]);
        yield return gm.WaitForTextCompletion("GunIntro3");

        EnemyScript[] e = FindObjectsOfType<EnemyScript>();
        ply.canMove = true;
        ply.canLaunchHand = true;
        ply.canShoot = true;

        enemiesDefeated = false;
        while(!enemiesDefeated)
        {
            bool stillAlive = false;
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i] != null)
                    stillAlive = true;
            }
            enemiesDefeated = !stillAlive;
            yield return null;
        }

        yield return new WaitForSeconds(1.25f);
        ply.Freeze();
        if (ply.stats.goo < 50)
            ply.stats.goo = 50;
        yield return gm.WaitForTextCompletion("HealIntro");
        Instantiate(itemsToSpawn[4], ply.transform.position, Quaternion.identity);
        ply.stats.health = 100;
        ply.ReceiveDamage(25);
        yield return new WaitForSeconds(1);
        yield return gm.WaitForTextCompletion("HealIntro2");
        yield return gm.WaitForTextCompletion("HealIntro3");
        ply.canMove = true;
        ply.canLaunchHand = false;
        ply.canShoot = false;
        while (ply.stats.health == 75)
            yield return null;

        yield return new WaitForSeconds(1f);
        ply.Freeze();
        yield return gm.WaitForTextCompletion("HealFollowup");
        SpawnObject(spawnPoints[0].position, itemsToSpawn[5]);
        yield return gm.WaitForTextCompletion("ChargeIntro");
        ply.canMove = true;
        ply.canLaunchHand = true;

        while (ply.heldObject == null)
            yield return null;

        EnemyScript e3 = ply.heldObject.GetComponent<EnemyScript>();
        ply.Freeze();
        yield return gm.WaitForTextCompletion("ShieldDemonstration");
        yield return new WaitForSeconds(0.5f);
        e3.ReceiveShieldDamage();
        yield return new WaitForSeconds(1f);
        e3.ReceiveShieldDamage();
        yield return new WaitForSeconds(1f);
        e3.ReceiveShieldDamage();
        yield return new WaitForSeconds(1.25f);
        yield return gm.WaitForTextCompletion("ShieldDemonstration2");
        ply.ForceRightClickRelease();
        ply.canLaunchHand = true;
        ply.canMove = true;

        while (e3 != null)
            yield return null;
        
        yield return new WaitForSeconds(1.5f);
        ply.Freeze();
        yield return gm.WaitForTextCompletion("ChargeDemonstration");
        SpawnObject(spawnPoints[0].position, itemsToSpawn[5]);
        yield return gm.WaitForTextCompletion("ChargeDemonstration2");
        ply.canMove = true;
        ply.canLaunchHand = true;

        EnemyScript e4 = FindObjectOfType<EnemyScript>();

        bool brokeEnemyCorrectly = false;
        while (!brokeEnemyCorrectly)
        {
            while(e4 != null)
            {
                if (e4.stats.currentShieldValue == 1)
                    brokeEnemyCorrectly = true;
                yield return null;
            }

            if(!brokeEnemyCorrectly)
            {
                yield return new WaitForSeconds(0.5f);
                ply.Freeze();
                SpawnObject(spawnPoints[0].position, itemsToSpawn[5]);
                yield return gm.WaitForTextCompletion("ChargeRetry");
                e4 = FindObjectOfType<EnemyScript>();
                ply.canMove = true;
            }
        }

        yield return new WaitForSeconds(1.5f);
        ply.Freeze();
        yield return gm.WaitForTextCompletion("TutorialEnd");
        Destroy(stairsBlocker.gameObject);
        ply.canMove = true;

        while (!exitedDoor)
            yield return null;

        ply.Freeze();
        yield return gm.FadeToBlack();
        FindObjectOfType<CameraController>().target = GameObject.Find("CameraPositionTarget").transform;
        yield return new WaitForSeconds(0.5f);
        yield return gm.FadeFromBlack();
        yield return new WaitForSeconds(0.5f);
        camAnimator.Play("CarCameraPan");
        yield return new WaitForSeconds(7f);
        yield return gm.WaitForTextCompletion("CarDialog");
        camAnimator.Play("CarCameraExit");
        yield return new WaitForSeconds(3f);
        gm.LoadLevel(1);
    }

    IEnumerator WaitForMovement()
    {
        while (!(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", 1);
            exitedDoor = true;
        }
    }
}
