using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyScript : EnemyScript
{
    public GameObject projectile;
    public GameObject bigExplosion;
    bool shooting;
    bool dead;
    int storedHealth;
    string animationSuffix;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        storedHealth = stats.health;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update animations based on how much health we have left
        if (stats.health < stats.maxHealth * 0.33f)
            animationSuffix = "Damaged";
        else if (stats.health < stats.maxHealth * 0.66f)
            animationSuffix = "MidHealth";
        else
            animationSuffix = "Healthy";

        if (!shooting && storedHealth > stats.health)
        {
            anim.Play("BossHurt_" + animationSuffix, -1, 0);
            storedHealth = stats.health;
        }

        if (stats.health <= 0 && !dead)
        {
            StopAllCoroutines();
            anim.Play("BossDie");
            dead = true;
            StartCoroutine(gm.BossPhase1DieCutscene());
        }
    }

    public override void UpdateMovement()
    {
        StartCoroutine(ShootPatterns());
    }

    IEnumerator ShootPatterns()
    {
        shooting = false;
        anim.Play("BossIdle_" + animationSuffix, -1, 0);
        yield return new WaitForSeconds(5f);
        shooting = true;
        for (int i = 0; i < 3; i++)
        {
            anim.Play("BossShoot_"+animationSuffix, -1, 0);
            gm.PlaySFX(gm.generalSfx[9]);
            yield return new WaitForSeconds(0.5f);
        }

        if(ply.stats.health > 0)
            StartCoroutine(ShootPatterns());
    }

    public void SpawnProjectile()
    {
        Instantiate(projectile, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
    }
}
