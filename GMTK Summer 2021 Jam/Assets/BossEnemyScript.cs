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
        yield return new WaitForSeconds(6f);
        for (int i = 0; i < 3; i++)
        {
            anim.Play("BossShoot", -1, 0);
            gm.PlaySFX(gm.generalSfx[9]);
            for (int j = 0; j < 2; j++)
            {
                Instantiate(projectile, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(ShootPatterns());
    }
}
