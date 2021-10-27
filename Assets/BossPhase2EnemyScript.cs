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
    int currentPathIndex = -1;
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

        // Update animations based on how much health we have left
        if (stats.health < stats.maxHealth * 0.25f)
        {
            if (currentPathIndex != 2)
            {
                animationSuffix = "NearDeath";
                SetPath(2);
            }
        }
        else if (stats.health < stats.maxHealth * 0.5f)
        {
            if (currentPathIndex != 2)
            {
                animationSuffix = "Damaged";
                SetPath(2);
            }
        }
        else if (stats.health < stats.maxHealth * 0.75f)
        {
            if (currentPathIndex != 1)
            {
                animationSuffix = "MidHealth";
                SetPath(1);
            }
        }
        else if (currentPathIndex != 0)
        {
            animationSuffix = "Healthy";
            SetPath(0);
        }
    }

    public void Freeze()
    {

    }

    IEnumerator GooDropSpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        int gooChance = Random.Range(0, 3);
        if(gooChance > 0)
            Instantiate(splatterDrops[Random.Range(0, 2)], transform.position, Quaternion.identity);
        else
            Instantiate(gooDrops[Random.Range(0, 2)], transform.position, Quaternion.identity);
        StartCoroutine(GooDropSpawnLoop());
    }

    void SetPath(int index)
    {
        print("Moved to path " + index);
        bpm.SetNodePath(index);
        currentNode = -1;
        currentPathIndex = index;
        stats.pathSpeedMultiplier = 2 + index;
        anim.SetFloat("ShootSpeed", Mathf.Clamp((currentPathIndex * 0.25f) + 1, 1, 2));
    }

    IEnumerator CombatLoop()
    {
        // Attacks:
        // 0 - Bullet-firing laser
        // 1 - Scum Skull circle pattern
        // 2 - Slow moving snot bubbles

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
                    yield return SnotBallAttack();
                    break;
            }

            yield return new WaitForSeconds(2f / Mathf.Clamp(currentPathIndex, 1, 3));
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

            //anim.Play("BossShoot_" + animationSuffix, -1, 0);
            SpawnCurrentProjectiles();
            yield return new WaitForSeconds(3.5f / Mathf.Clamp(currentPathIndex, 1, 3));
        }
        yield return null;
    }

    IEnumerator SnotBallAttack()
    {
        for (int i = 0; i < 8*Mathf.Clamp(1 + currentPathIndex*0.25f, 1, 3); i++)
        {
            anim.Play("BossShoot_" + animationSuffix, -1, 0);
            yield return new WaitForSeconds(0.5f / Mathf.Clamp(currentPathIndex, 1, 3));
        }
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
        else if (currentAttack == 1)
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
        else if(currentAttack == 2)
        {
            SnotProjectile s = Instantiate(projectiles[3], transform.position + headHoleOffset, Quaternion.identity).GetComponent<SnotProjectile>();
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
        Vector3 vel = (ply.transform.position - transform.position).normalized * 40;
        ply.additionalForce = vel;

        while (ply.additionalForce.magnitude > 0.25f)
            yield return new WaitForFixedUpdate();
        
        ply.additionalForce = Vector2.zero;
        knockingPlayerBack = false;
    }
}
