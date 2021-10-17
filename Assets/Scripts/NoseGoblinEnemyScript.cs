using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoseGoblinEnemyScript : EnemyScript
{
    public bool shooting;
    public GameObject projectile;
    float randReloadSpeed;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        randReloadSpeed = Random.Range(stats.damageCooldown * 0.5f, stats.damageCooldown * 2);
        rechargingAttack = true;
        StartCoroutine(Reload());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetWalkSpeed();

        if (!ply.isDying)
            UpdateMovement();
        else
            StopMoving();

        CheckIfHeld();
    }

    public override void UpdateMovement()
    {
        RaycastForPlayer();

        if (!shooting)
        {
            if (noticedPlayer && ply.stats.health > 0)
            {
                if (!rechargingAttack && !g.isHeld)
                {
                    shooting = true;
                    Vector3 inaccuracy = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * stats.inaccuracyMultiplier;
                    CheckAndPlayClip(stats.animationPrefix + "_Shoot" + GetCompassPointFromAngle(AngleBetween(ply.transform.position + inaccuracy)));
                    rechargingAttack = true;
                }
                else if (Vector3.Distance(transform.position, ply.transform.position) < 7)
                    MoveAwayFromPlayer();
                else
                    MoveTowardsPlayer();
            }
            else
                FollowPath();
        }
    }

    public void LaunchProjectile()
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
        rb.velocity = (transform.position - ply.transform.position).normalized * 2;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0.2f);
        shooting = false;

        yield return new WaitForSeconds(Mathf.Clamp(randReloadSpeed, 0, 5));
        rechargingAttack = false;
    }
}
