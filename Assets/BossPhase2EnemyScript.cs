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

    int currentAttack = -1;
    string animationSuffix = "Healthy";

    Animator bossLaser;
    Rigidbody2D prb;

    Vector3 initialPosition;
    Vector3 headHoleOffset = Vector3.up * 1.75f;

    IEnumerator combatCoroutine;

    BossPhase2Manager bpm;

    bool knockingPlayerBack;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        spr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        prb = ply.GetComponent<Rigidbody2D>();
        bpm = FindObjectOfType<BossPhase2Manager>();
        bossLaser = transform.GetChild(0).GetComponent<Animator>();
        initialPosition = transform.position;
        combatCoroutine = CombatLoop();
        StartCoroutine(combatCoroutine);
        StartCoroutine(GooDropSpawnLoop());
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    IEnumerator GooDropSpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        Instantiate(splatterDrops[Random.Range(0, 2)], transform.position, Quaternion.identity);
        StartCoroutine(GooDropSpawnLoop());
    }

    IEnumerator CombatLoop()
    {
        // Attacks:
        // 0 - Bullet-firing laser
        // 1 - Scum Skull circle pattern
        // 2 - Slow moving snot bubbles

        bpm.SetNodePath(0);
        currentNode = -1;
        while (true)
        {
            int lastAttack = currentAttack;
            while (currentAttack == lastAttack)
                currentAttack = Random.Range(0, 3);

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

            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator LaserAttack()
    {
        gm.PlaySFX(gm.generalSfx[20]);
        yield return new WaitForSeconds(0.1f);
        bossLaser.Play("BossLaser", -1, 0);
        yield return new WaitForSeconds(2);
    }

    IEnumerator GhostBulletsAttack()
    {
        for (int i = 0; i < 4; i++)
        {
            print(i);
            if ((i + 1) % 2 == 0)
                projectileAngleOffset = Mathf.PI / 4;
            else
                projectileAngleOffset = 0;

            anim.Play("BossShoot_" + animationSuffix, -1, 0);
            yield return new WaitForSeconds(2f);
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
        FollowPath();
    }

    public void SpawnCurrentProjectiles()
    {
        StartCoroutine(SpawnCurrentProjectilesCoroutine());
    }

    IEnumerator SpawnCurrentProjectilesCoroutine()
    {
        if (currentAttack == 0)
        {
            for (int i = 0; i < 40; i++)
            {
                float rand = Random.Range(3.75f, 10);
                Vector2 spawnPos = transform.position + Vector3.up * rand;
                int dir = Random.Range(0, 2);
                if (dir == 0)
                    dir = -1;
                Vector3 vel = new Vector3(dir, Random.Range(-1, 1f) * 0.65f).normalized;
                Quaternion lookAt = Quaternion.LookRotation(transform.forward, vel);
                EnemyProjectile e = Instantiate(projectiles[0], spawnPos, Quaternion.identity).GetComponent<EnemyProjectile>();
                e.transform.rotation = lookAt;
                e.Launch(vel * e.speed);
                //gm.PlaySFXStoppable(gm.generalSfx[2], Random.Range(0.8f, 1.2f));
                yield return new WaitForSeconds(0.05f);
            }
        }
        if (currentAttack == 1)
        {
            int sides = 8;
            for (int i = 0; i < sides; i++)
            {
                float val = (i / (float)sides * 2 * Mathf.PI) + projectileAngleOffset;
                Vector3 pos = new Vector2(Mathf.Cos(val), Mathf.Sin(val)).normalized;

                int enemy = 2;
                if (i % 2 == 0)
                    enemy = 1;

                ScumSkullEnemyScript s = Instantiate(projectiles[enemy], transform.position + pos + headHoleOffset, Quaternion.identity).GetComponent<ScumSkullEnemyScript>();
                s.targetVelocity = (Vector2)pos * 10;
                //yield return new WaitForSeconds(0.1f);
                //gm.PlaySFXStoppable(gm.generalSfx[3], 1);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !knockingPlayerBack)
        {
            knockingPlayerBack = true;
            StartCoroutine(PlayerKnockback());
        }
    }

    IEnumerator PlayerKnockback()
    {
        ply.ReceiveDamage(stats.damage);
        ply.Freeze();
        Vector3 vel = (ply.transform.position - transform.position).normalized * 25;
        prb.velocity = vel;

        while (prb.velocity.magnitude > 0.25f)
        {
            prb.velocity = Vector2.Lerp(prb.velocity, Vector2.zero, 0.06f);
            yield return new WaitForFixedUpdate();
        }
        prb.velocity = Vector2.zero;
        ply.canMove = true;
        knockingPlayerBack = false;
    }
}
