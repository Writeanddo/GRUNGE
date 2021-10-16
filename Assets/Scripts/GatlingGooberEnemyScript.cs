using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGooberEnemyScript : EnemyScript
{
    public GameObject projectile;

    public Transform[] handPositions;

    // Start is called before the first frame update
    void Start()
    {
        //rechargingAttack = true;
        GetReferences();
        gibSpawnYOffset = -1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetWalkSpeed();

        if (!ply.isDying)
            UpdateMovement();
        else
            StopMoving();
    }

    public override void UpdateMovement()
    {
        RaycastForPlayer();
        if (noticedPlayer && ply.stats.health > 0)
            MoveTowardsPlayer();
        else
            FollowPath();

        if (!rechargingAttack && noticedPlayer)
        {
            rechargingAttack = true;
            StartCoroutine(LaunchProjectile());
        }
    }

    public IEnumerator LaunchProjectile()
    {
        for (int i = 0; i < 3; i++)
        {
            gm.PlaySFX(gm.generalSfx[10]);
            Instantiate(projectile, handPositions[0].position, Quaternion.identity);
            rb.velocity = (transform.position - ply.transform.position).normalized * 2f;
            yield return new WaitForSeconds(0.25f);
            gm.PlaySFX(gm.generalSfx[10]);
            Instantiate(projectile, handPositions[1].position, Quaternion.identity);
            rb.velocity = (transform.position - ply.transform.position).normalized * 2f;
            yield return new WaitForSeconds(0.75f);
        }
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(stats.damageCooldown);
        rechargingAttack = false;
    }
}
