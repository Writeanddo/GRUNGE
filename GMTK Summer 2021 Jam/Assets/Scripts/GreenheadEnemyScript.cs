using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenheadEnemyScript : EnemyScript
{
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

        CheckIfHeld();
    }

    public override void UpdateMovement()
    {
        RaycastForPlayer();
        if (noticedPlayer && ply.stats.health > 0)
            MoveTowardsPlayer();
        else
            FollowPath();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !rechargingAttack)
        {
            rechargingAttack = true;
            ply.ReceiveDamage(stats.damage);
            StartCoroutine(DealDamage());
        }
    }

    IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(stats.damageCooldown);
        rechargingAttack = false;
    }
}
