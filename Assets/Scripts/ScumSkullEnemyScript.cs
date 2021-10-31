using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScumSkullEnemyScript : EnemyScript
{
    public Vector2 targetVelocity;
    bool hasBeenHeld;
    float delta;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        StartCoroutine(MovementPath1());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfHeld();
        if (g.isHeld && !hasBeenHeld)
        {
            hasBeenHeld = true;
        }
    }

    IEnumerator MovementPath1()
    {
        while (targetVelocity == Vector2.zero)
            yield return null;

        rb.velocity = targetVelocity;
        FaceTowardsVelocity();
        yield return new WaitForSeconds(0.75f);

        while (delta < 720)
        {
            Vector2 offset = rb.velocity + Vector2.Perpendicular(rb.velocity)*0.1f;
            offset = Vector2.ClampMagnitude(offset, targetVelocity.magnitude);

            delta += Vector2.Angle(rb.velocity, offset);

            if (!hasBeenHeld)
                rb.velocity = offset;

            FaceTowardsVelocity();
            yield return new WaitForFixedUpdate();
        }
    }
    public override void UpdateMovement()
    {
        if (targetVelocity == Vector2.zero)
            return;
        
    }

    IEnumerator IsHeldCharge()
    {
        gm.PlaySFX(gm.generalSfx[21]);
        yield return new WaitForSeconds(4);
        if(g.isHeld)
            ReceiveShieldDamage();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!g.isHeld && collision.tag == "Player")
        {
            if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
            {
                ply.heldObject.GetComponent<EnemyScript>().ReceiveShieldDamage();
            }
            else
                ply.ReceiveDamage(stats.damage);
            ExplodeBig();
        }

        
    }
}
