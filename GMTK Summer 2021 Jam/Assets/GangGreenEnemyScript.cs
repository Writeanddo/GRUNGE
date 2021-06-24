using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangGreenEnemyScript : EnemyScript
{
    public LineRenderer[] connectedLines;
    public int[] connectedLinePoints;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
    }

    void FixedUpdate()
    {
        SetWalkSpeed();
        UpdateConnectedRopes();

        if (!ply.isDying)
            UpdateMovement();
        else
            StopMoving();

        CheckIfHeld();
    }

    public void UpdateConnectedRopes()
    {
        for(int i = 0; i < connectedLines.Length; i++)
        {
            if (connectedLines[i] != null)
                connectedLines[i].SetPosition(connectedLinePoints[i], transform.position);
        }
    }

    private void Update()
    {
        if (stats.health <= 0)
            foreach (LineRenderer l in connectedLines)
                Destroy(l.gameObject);
    }

    public override void UpdateMovement()
    {
        RaycastForPlayer();
        if (noticedPlayer && ply.stats.health > 0)
            MoveTowardsPlayer();
        else
            FollowPath();
    }
}
