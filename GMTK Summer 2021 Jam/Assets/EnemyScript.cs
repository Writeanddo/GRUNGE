using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int health = 30;
        public int damage = 2;
        public float damageCooldown = 2;
        public float noticePlayerDistance = 15;
        public int numGooDrops = 5;
    }

    public EnemyStats stats;
    public bool noticedPlayer;
    public Transform[] idlePath;
    public GameObject[] gooDrops;

    Animator anim;
    SpriteRenderer spr;
    Rigidbody2D rb;
    PlayerController ply;
    GameManager gm;
    EnemyScript raycastHitEnemy;

    bool rechargingAttack;
    float randSpeedMultiplier;
    float timeSinceLastSeenPlayer;
    int currentNode;
    int pathDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        randSpeedMultiplier = Random.Range(1.5f, 2.25f);
        currentNode = GetNearestNode();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        RaycastForPlayer();

        if (noticedPlayer)
        {
            Vector2 dir = (ply.transform.position - transform.position).normalized;
            rb.velocity = Vector2.Lerp(rb.velocity, dir * randSpeedMultiplier * 1.75f, 0.1f);
            CheckAndPlayClip("Greenhead_Walk" + GetCompassPointFromAngle(AngleBetween(ply.transform.position)));
        }
        else
        {
            // Move towards nearest node
            Vector2 dir = (idlePath[currentNode].transform.position - transform.position).normalized;
            rb.velocity = Vector2.Lerp(rb.velocity, dir * randSpeedMultiplier, 0.1f);
            CheckAndPlayClip("Greenhead_Walk" + GetCompassPointFromAngle(AngleBetween(idlePath[currentNode].position)));

            if (Vector2.Distance(transform.position, idlePath[currentNode].position) < 0.5f)
                currentNode = GetNextNode();
        }
    }

    int GetNearestNode()
    {
        int closestNode = 0;
        float storedLength = 1000;

        // Decide which way along the path we'll move
        int pathDir = Random.Range(0, 2);
        switch (pathDir)
        {
            case (0):
                pathDirection = -1;
                break;
            case (1):
                pathDir = 1;
                break;
        }

        // Raycast to all nodes
        for (int i = 0; i < idlePath.Length; i++)
        {
            Vector2 dir = (idlePath[i].position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 50);
            if (hit && storedLength > Vector2.Distance(transform.position, idlePath[i].position))
            {
                closestNode = i;
                storedLength = Vector2.Distance(transform.position, idlePath[i].position);
            }
        }
        return closestNode;
    }

    // Returns index of next node on path
    int GetNextNode()
    {
        if ((currentNode == 0 && pathDirection == -1) || (currentNode == idlePath.Length - 1 && pathDirection == 1))
            pathDirection *= -1;

        return currentNode + pathDirection;
    }

    float AngleBetween(Vector3 targetPosition)
    {
        Vector3 relative = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(-relative.x + Mathf.Sin(Time.deltaTime / 10) * 25, relative.y) * Mathf.Rad2Deg;
        return -angle;
    }

    void RaycastForPlayer()
    {
        Vector2 dir = (ply.transform.position - transform.position).normalized;

        float scanDistance = stats.noticePlayerDistance;
        if (noticedPlayer)
            scanDistance = 50;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, scanDistance);
        if (hit)
        {
            if (hit.transform.tag == "Player")
            {
                timeSinceLastSeenPlayer = 0;
                noticedPlayer = true;

                // Inform nearby enemies
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.tag == "Enemy" || hits[i].transform.tag == "GrabbableEnemy")
                        hits[i].transform.GetComponent<EnemyScript>().noticedPlayer = true;
                }

            }
            else if (hit.transform.tag == "Enemy" || hit.transform.tag == "GrabbableEnemy")
            {
                if (raycastHitEnemy == null || (raycastHitEnemy.transform != hit.transform))
                    raycastHitEnemy = hit.transform.GetComponent<EnemyScript>();

                if (raycastHitEnemy.noticedPlayer)
                    noticedPlayer = true;
            }
            else if (noticedPlayer)
            {
                timeSinceLastSeenPlayer += Time.deltaTime;
                if (timeSinceLastSeenPlayer > 2)
                {
                    currentNode = GetNearestNode();
                    noticedPlayer = false;
                }
            }

        }
    }

    string GetCompassPointFromAngle(float angle)
    {
        // -90 = W, 0 = N, 90 = E, 180/-180 = S

        if (angle >= -180 && angle < -165)
            return "S";
        else if (angle >= -165 && angle < -105)
            return "SW";
        else if (angle >= -105 && angle < -65)
            return "W";
        else if (angle >= -65 && angle < -25)
            return "NW";
        else if (angle >= -25 && angle < 25)
            return "N";
        else if (angle >= 25 && angle < 65)
            return "NE";
        else if (angle >= 65 && angle < 105)
            return "E";
        else if (angle >= 105 && angle < 165)
            return "SE";
        else
            return "S";
    }

    public void ReceiveDamage(int damage)
    {
        StartCoroutine(ReceiveDamageCoroutine(damage));
    }

    IEnumerator ReceiveDamageCoroutine(int damage)
    {
        spr.color = Color.red;
        float shakeAmount = 1;
        stats.health -= damage;

        if (stats.health <= 0)
            ExplodeIntoGoo();

        while (spr.color.g < 0.9f)
        {
            spr.color = Color.Lerp(spr.color, Color.white, 0.1f);
            spr.transform.localPosition = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
            shakeAmount /= 1.5f;
            yield return new WaitForFixedUpdate();
        }
        spr.transform.localPosition = Vector2.zero;
    }


    public void CheckAndPlayClip(string clipName)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }

    public void ExplodeIntoGoo()
    {
        gm.ScreenShake(6);
        for (int i = 0; i < stats.numGooDrops; i++)
        {
            Instantiate(gooDrops[Random.Range(0, gooDrops.Length)], transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
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
