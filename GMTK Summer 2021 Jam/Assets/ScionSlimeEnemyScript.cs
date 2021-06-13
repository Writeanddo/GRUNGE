using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScionSlimeEnemyScript : EnemyScript
{
    bool shooting;
    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        rechargingAttack = true;
        StartCoroutine(Reload());
    }

    public override void UpdateMovement()
    {
        FollowPath();
    }

    private void FixedUpdate()
    {
        UpdateMovement();

        if (!shooting && !rechargingAttack)
        {
            LaunchProjectiles();
            rechargingAttack = true;
        }
    }

    public void LaunchProjectiles()
    {
        // Shoot three bullets
        for (int i = 0; i < 3; i++)
        {
            EnemyProjectile p = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
            p.Launch(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * p.speed);
        }
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0.2f);
        shooting = false;

        yield return new WaitForSeconds(stats.damageCooldown);
        rechargingAttack = false;
    }
}
