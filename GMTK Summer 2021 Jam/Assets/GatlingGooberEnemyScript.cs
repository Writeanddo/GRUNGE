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

        GetReferences();
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
        gm.PlaySFX(gm.generalSfx[10]);
        Instantiate(projectile, handPositions[0].position, Quaternion.identity);
        rb.velocity = (transform.position - ply.transform.position).normalized * 1.5f;
        yield return new WaitForSeconds(0.8f);
        gm.PlaySFX(gm.generalSfx[10]);
        Instantiate(projectile, handPositions[1].position, Quaternion.identity);
        rb.velocity = (transform.position - ply.transform.position).normalized * 1.5f;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(stats.damageCooldown);
        rechargingAttack = false;
    }
}
