using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScumSkullEnemyScript : EnemyScript
{
    public Vector2 targetVelocity;
    bool hasBeenHeld;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfHeld();
        if (g.isHeld)
            hasBeenHeld = true;

        UpdateMovement();
    }

    public override void UpdateMovement()
    {
        if (targetVelocity == Vector2.zero)
            return;

        float dist = Vector2.Distance(transform.position, ply.transform.position);
        if(!hasBeenHeld)
            rb.velocity = targetVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!g.isHeld && collision.tag == "Player")
        {
            ply.ReceiveDamage(stats.damage);
            ExplodeBig();
        }
    }
}
