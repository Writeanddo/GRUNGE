using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScumSkullEnemyScript : EnemyScript
{
    public Vector2 targetVelocity;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
    }

    public override void UpdateMovement()
    {
        if (targetVelocity == Vector2.zero)
            return;

        rb.velocity = targetVelocity;
    }
}
