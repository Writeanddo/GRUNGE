using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyScript : EnemyScript
{
    public GameObject projectile;
    bool shooting;
    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(stats.health <= 0 && !dead)
        {
            StopAllCoroutines();
            anim.Play("BossDie");
            dead = true;
            gm.LevelOverSequenece();
        }
    }

    public override void UpdateMovement()
    {
        StartCoroutine(ShootPatterns());
    }

    IEnumerator ShootPatterns()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 3; i++)
        {
            anim.Play("BossShoot_Healthy", -1, 0);
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
