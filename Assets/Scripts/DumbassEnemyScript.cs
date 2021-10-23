using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbassEnemyScript : EnemyScript
{
    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfHeld();

        if (!g.isHeld && g.canBreak)
            rb.velocity = Vector2.zero;
    }

    public override void UpdateMovement()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    IEnumerator DealDamage()
    {
        yield return null;
    }
}