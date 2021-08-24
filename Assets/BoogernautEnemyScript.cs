using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogernautEnemyScript : EnemyScript
{
    Transform laserOrigin;
    Transform laserImpact;
    SpriteRenderer laser;

    // Start is called before the first frame update
    void Start()
    {
        laserOrigin = transform.GetChild(1);
        laser = laserOrigin.transform.GetChild(0).GetComponent<SpriteRenderer>();
        laserImpact = laserOrigin.GetChild(1);
        GetReferences();
        StartCoroutine(LaserDamage());
    }

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
        {
            // Move away when firing laser
            //if(!rechargingAttack)
                //MoveAwayFromPlayer();
            //else
                MoveTowardsPlayer();
        }
        else
            FollowPath();
    }

    IEnumerator LaserDamage()
    {
        

        while (true)
        {
            // Get initial angle
            Vector3 diff = ply.transform.position - laserOrigin.transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            laserOrigin.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

            Vector2 hitPos;
            int layerMask = 1 << 14;
            RaycastHit2D hit = Physics2D.Raycast(laserOrigin.transform.position, laserOrigin.transform.right, 100, layerMask);
            Debug.DrawLine(laserOrigin.transform.position, hit.point, Color.blue);
            hitPos = laser.transform.InverseTransformDirection(hit.point);

            laserImpact.transform.position = new Vector2(hit.point.x, hit.point.y + 0.191f);
            laser.transform.localPosition = new Vector2(Vector3.Distance(laserOrigin.transform.position, hit.point) / 2, laser.transform.localPosition.y);
            laser.size = new Vector2(laser.transform.localPosition.x * 2, laser.size.y);

            int playerLayerMask = 1 << 8;
            RaycastHit2D playerHit = Physics2D.CircleCast(laserOrigin.transform.position, 0.5f, laserOrigin.transform.right, 100, playerLayerMask);

            if (playerHit.transform != null)
            {

            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void DeactivateLaser()
    {
        laser.color = Color.clear;
        //laserImpact.color = Color.clear;
        laserImpact.GetComponent<AudioSource>().volume = 0;
        //laserActive = false;
    }
}
