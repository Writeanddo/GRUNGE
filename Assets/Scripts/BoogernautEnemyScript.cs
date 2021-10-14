using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogernautEnemyScript : EnemyScript
{
    Transform laserOrigin;
    Transform laserImpact;
    SpriteRenderer laser;
    AudioSource sfx;

    public LayerMask laserHitMask;
    public LayerMask damageHitMask;
    public bool attacking;
    public bool isDamaged;

    bool rotatingLaser;

    
    // Start is called before the first frame update
    void Start()
    {
        laserOrigin = transform.GetChild(1);
        laser = laserOrigin.transform.GetChild(0).GetComponent<SpriteRenderer>();
        laserImpact = laserOrigin.GetChild(1);
        sfx = GetComponentInChildren<AudioSource>();
        GetReferences();
    }

    void FixedUpdate()
    {
        SetWalkSpeed();

        if (!ply.isDying)
            UpdateMovement();
        else
            StopMoving();

        // Update laser sorting order if attacking
        if (attacking)
        {
            string dir = GetCompassPointFromAngle(AngleBetween(ply.transform.position));
            if (dir == "N" || dir == "NE" || dir == "NW")
                laser.sortingOrder = spr.sortingOrder - 1;
            else
                laser.sortingOrder = spr.sortingOrder + 1;
        }
    }

    public override void UpdateMovement()
    {
        RaycastForPlayer();
        if (!attacking)
        {
            if (noticedPlayer && ply.stats.health > 0)
            {
                if(!rechargingAttack)
                    StartCoroutine(LaserDamage());
                else
                    MoveTowardsPlayer();
            }
            else
                FollowPath();
        }
        else
            StopMoving();
    }

    IEnumerator LaserDamage()
    {
        bool hitPlayer = false;
        attacking = true;

        // Get initial angle
        Vector3 diff = ply.transform.position - laserOrigin.transform.position;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rotatingLaser = true;
        StartCoroutine(RotateLaser(rot_z - 45));

        while (rotatingLaser)
        {
            Vector2 hitPos;
            RaycastHit2D hit = Physics2D.Raycast(laserOrigin.transform.position, laserOrigin.transform.right, 50, laserHitMask);
            Debug.DrawLine(laserOrigin.transform.position, hit.point, Color.blue);
            hitPos = laser.transform.InverseTransformDirection(hit.point);

            laserImpact.transform.position = new Vector2(hit.point.x, hit.point.y + 0.191f);
            laser.transform.localPosition = new Vector2(Vector3.Distance(laserOrigin.transform.position, hit.point) / 2, laser.transform.localPosition.y);
            laser.size = new Vector2(laser.transform.localPosition.x * 2, laser.size.y);

            if (!hitPlayer)
            {
                RaycastHit2D playerHit = Physics2D.CircleCast(laserOrigin.transform.position, 0.1f, laserOrigin.transform.right, Vector3.Distance(laserOrigin.transform.position, hit.point), damageHitMask);

                if (playerHit.transform != null)
                {
                    print("Hit player");
                    if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
                        ply.heldObject.GetComponent<EnemyScript>().ReceiveShieldDamage();
                    else
                        ply.ReceiveDamage(13);
                    hitPlayer = true;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        laser.size = new Vector2(0, laser.size.y);

        StartCoroutine(Reload());
        attacking = false;
    }

    IEnumerator Reload()
    {
        rechargingAttack = true;
        yield return new WaitForSeconds(stats.damageCooldown);
        rechargingAttack = false;
    }

    IEnumerator RotateLaser(float zAngle)
    {
        if (zAngle < 0)
            zAngle += 360;

        laserOrigin.transform.rotation = Quaternion.Euler(0f, 0f, zAngle);
        sfx.PlayOneShot(gm.generalSfx[13]);
        //yield return new WaitForSeconds(0.33f);
        float amountRotated = 0f;
        float timePassed = 0;
        while (amountRotated < 90)
        {
            laserOrigin.transform.rotation = Quaternion.Euler(0f, 0f, laserOrigin.transform.rotation.eulerAngles.z + 1.125f);
            amountRotated += 1.125f;
            yield return new WaitForFixedUpdate();
            timePassed += Time.fixedDeltaTime;
        }
        rotatingLaser = false;
    }
}
