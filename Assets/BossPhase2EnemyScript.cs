using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase2EnemyScript : EnemyScript
{
    public GameObject[] projectiles;
    float movementTime;
    float projectileAngleOffset;

    public float lastFrameXPos;
    public float lastFrameYPos;
    float maxPosChange = 0.2f;

    int currentAttack;
    string animationSuffix = "Healthy";

    Animator bossLaser;

    Vector3 initialPosition;
    Vector3 headHoleOffset = Vector3.up * 1.75f;

    IEnumerator combatCoroutine;

    BossPhase2Manager bpm;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        bpm = FindObjectOfType<BossPhase2Manager>();
        bossLaser = transform.GetChild(0).GetComponent<Animator>();
        initialPosition = transform.position;
        combatCoroutine = CombatLoop();
        StartCoroutine(combatCoroutine);
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    IEnumerator CombatLoop()
    {
        // Attacks:
        // 0 - Bullet-firing laser
        // 1 - Scum Skull circle pattern
        // 2 - Slow moving snot bubbles

        currentNode = -1;
        bpm.SetNodePath(0);
        while (true)
        {
            currentAttack = 0;//Random.Range(0, 3);
            switch (currentAttack)
            {
                case 0:
                    yield return LaserAttack();
                    break;
                case 1:
                    yield return GhostBulletsAttack();
                    break;
                case 2:
                    break;
            }

            
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator LaserAttack()
    {
        gm.PlaySFX(gm.generalSfx[20]);
        yield return new WaitForSeconds(0.1f);
        bossLaser.Play("BossLaser", -1, 0);
        yield return new WaitForSeconds(3);

    }

    IEnumerator GhostBulletsAttack()
    {
        for(int i = 0; i < 5; i++)
        {
            if ((i + 1) % 2 == 0)
                projectileAngleOffset = Mathf.PI / 2;
            else
                projectileAngleOffset = 0;

            anim.Play("BossShoot_" + animationSuffix, -1, 0);
            yield return new WaitForSeconds(14f);
        }
        yield return null;
    }

    IEnumerator SnotBallAttack()
    {
        yield return null;
    }


    float MCos(float value)
    {
        return Mathf.Cos(Mathf.Deg2Rad * value);
    }

    float MSin(float value)
    {
        return Mathf.Sin(Mathf.Deg2Rad * value);
    }

    public override void UpdateMovement()
    {
        //FollowPath();
    }

    public void SpawnCurrentProjectiles()
    {
        StartCoroutine(SpawnCurrentProjectilesCoroutine());
    }

    IEnumerator SpawnCurrentProjectilesCoroutine()
    {
        if (currentAttack == 0)
        {
            for (int i = 0; i < 9; i++)
            {
                float rand = Random.Range(3.75f, 10);
                Vector2 spawnPos = transform.position + Vector3.up * rand;
                int dir = Random.Range(0, 2);
                if (dir == 0)
                    dir = -1;
                Vector3 vel = new Vector3(dir, Random.Range(-1, 1f)*0.75f).normalized;
                Quaternion lookAt = Quaternion.LookRotation(transform.forward, vel);
                EnemyProjectile e = Instantiate(projectiles[0], spawnPos, Quaternion.identity).GetComponent<EnemyProjectile>();
                e.transform.rotation = lookAt;
                e.Launch(vel*e.speed);
                //gm.PlaySFXStoppable(gm.generalSfx[2], Random.Range(0.8f, 1.2f));
                yield return new WaitForSeconds(0.24f);
            }
        }
        if (currentAttack == 1)
        {
            for (int j = 0; j < 4; j++)
            {
                int sides = 8;
                for (int i = 0; i < sides; i++)
                {

                    float val = (i / (float)sides * 2 * Mathf.PI) + projectileAngleOffset;
                    Vector3 pos = new Vector2(Mathf.Cos(val), Mathf.Sin(val)).normalized;
                    ScumSkullEnemyScript s = Instantiate(projectiles[1], transform.position + pos + headHoleOffset, Quaternion.identity).GetComponent<ScumSkullEnemyScript>();
                    s.targetVelocity = (Vector2)pos * 6;
                    //yield return new WaitForSeconds(0.1f);
                    //gm.PlaySFXStoppable(gm.generalSfx[3], 1);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
