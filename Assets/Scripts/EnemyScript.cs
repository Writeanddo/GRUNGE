using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyScript : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int health = 30;
        [HideInInspector]
        public int maxHealth;
        public bool overrideDeath;
        public int currentShieldValue = 4;
        public int damage = 2;
        public float pathSpeedMultiplier = 1;
        public float damageCooldown = 2;
        public float inaccuracyMultiplier = 1;
        public float noticePlayerDistance = 15;
        public float noticedSpeedMultiplier = 2;
        public int numGooDrops = 5;
        public float movementAccuracy = 0.1f;
        public float animationWalkSpeedMultiplier = 0.5f;
        public string animationPrefix;
        public bool useDirctionalAnimation = true;
    }

    public EnemyStats stats;
    public bool noticedPlayer;
    public GameObject[] gooDrops;
    public GameObject[] splatterDrops;
    public GameObject[] nonRandomDrops;

    public GameObject enemyExplosion;

    public LayerMask playerScanIgnoreMask;

    [HideInInspector]
    public Grabbable g;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public SpriteRenderer spr;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public PlayerController ply;
    [HideInInspector]
    public GameManager gm;
    [HideInInspector]
    public EnemyScript raycastHitEnemy;
    [HideInInspector]
    public EnemyWaveManager waves;
    [HideInInspector]
    public float gibSpawnYOffset = 0;

    public bool rechargingAttack;
    float randSpeedMultiplier;
    float timeSinceLastSeenPlayer;
    int currentNode = -1;
    int pathDirection = 1;
    bool readyToExplode = false;

    GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CheckIfHeld()
    {
        if (g.isHeld)
        {
            if (stats.useDirctionalAnimation)
            {
                anim.SetFloat("WalkSpeed", 4);
                CheckAndPlayClip(stats.animationPrefix + "_Walk" + GetCompassPointFromAngle(AngleBetween(crosshair.transform.position)));
            }
            if (stats.currentShieldValue == 1 && !readyToExplode)
            {
                readyToExplode = true;
                StartCoroutine(FlashFromDamage());
            }
        }
    }

    public void StopMoving()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.1f);
    }

    public void SetWalkSpeed()
    {
        if (!g.isHeld && stats.useDirctionalAnimation)
            anim.SetFloat("WalkSpeed", rb.velocity.magnitude * stats.animationWalkSpeedMultiplier);
    }

    public abstract void UpdateMovement();

    public void GetReferences()
    {
        int layer1 = LayerMask.NameToLayer("ExplosiveProjectileNoHit");
        int layer2 = LayerMask.NameToLayer("Enemy");
        int layer3 = LayerMask.NameToLayer("Prop");
        int layer4 = LayerMask.NameToLayer("Gibs");
        int layer5 = LayerMask.NameToLayer("GibNoContact");
        int layer6 = LayerMask.NameToLayer("HeldItem");

        int mask1 = 1 << layer1;
        int mask2 = 1 << layer2;
        int mask3 = 1 << layer3;
        int mask4 = 1 << layer4;
        int mask5 = 1 << layer5;
        int mask6 = 1 << layer6;

        playerScanIgnoreMask = ~(mask1 | mask2 | mask3 | mask4 | mask5 | mask6);

        stats.maxHealth = stats.health;
        g = GetComponent<Grabbable>();
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        waves = FindObjectOfType<EnemyWaveManager>();
        randSpeedMultiplier = Random.Range(1.5f, 2.25f);
        currentNode = GetNearestNode();
        crosshair = GameObject.Find("Crosshair");
        //StartCoroutine(FlashFromDamage());
    }

    public void MoveTowardsPlayer()
    {
        if (!g.isBeingThrown && !g.isHeld)
        {
            Vector2 dir = (ply.transform.position - transform.position).normalized;
            rb.velocity = Vector2.Lerp(rb.velocity, dir * randSpeedMultiplier * stats.noticedSpeedMultiplier, stats.movementAccuracy);
            if (stats.useDirctionalAnimation)
                CheckAndPlayClip(stats.animationPrefix + "_Walk" + GetCompassPointFromAngle(AngleBetween(ply.transform.position)));
        }
    }

    public void MoveAwayFromPlayer()
    {
        if (!g.isBeingThrown && !g.isHeld)
        {
            Vector2 dir = (transform.position - ply.transform.position).normalized;
            rb.velocity = Vector2.Lerp(rb.velocity, dir * randSpeedMultiplier * stats.noticedSpeedMultiplier, stats.movementAccuracy);
            if (stats.useDirctionalAnimation)
                CheckAndPlayClip(stats.animationPrefix + "_Walk" + GetCompassPointFromAngle(AngleBetween(ply.transform.position)));
        }
    }

    public void GetThrown()
    {
        StartCoroutine(GetThrownCoroutine());
    }

    IEnumerator GetThrownCoroutine()
    {
        while (rb.velocity.magnitude > 0.5f)
        {
            if (!readyToExplode)
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.15f);
            else
                gameObject.layer = LayerMask.NameToLayer("ExplosiveProjectile");
            yield return new WaitForFixedUpdate();
        }

        stats.currentShieldValue = 4;
        g.isBeingThrown = false;
    }

    public IEnumerator FlashFromDamage()
    {
        spr.color = Color.red;
        while (true)
        {
            spr.transform.localPosition = new Vector2(Random.Range(-1, 1f), Random.Range(-1, 1f)) * 0.05f;
            float rand = Random.Range(0.2f, 1f);
            spr.color = new Color(spr.color.r, rand, rand);
            yield return new WaitForFixedUpdate();
        }
        //StartCoroutine(FlashFromDamage());
    }

    public void FollowPath()
    {
        if (!g.isBeingThrown && !g.isHeld)
        {
            if (currentNode == -1)
                currentNode = GetNearestNode();

            Vector2 dir = Vector2.zero;

            // Move towards nearest node
            if (waves.enemyPath.Length > 0)
                dir = ((Vector2)waves.enemyPath[currentNode].transform.position - (Vector2)transform.position).normalized;

            rb.velocity = Vector2.Lerp(rb.velocity, dir * randSpeedMultiplier * stats.pathSpeedMultiplier, stats.movementAccuracy);
            if (stats.useDirctionalAnimation)
                CheckAndPlayClip(stats.animationPrefix + "_Walk" + GetCompassPointFromAngle(AngleBetween(waves.enemyPath[currentNode].position)));

            if (Vector2.Distance(transform.position, waves.enemyPath[currentNode].position) < 0.5f)
                currentNode = GetNextNode();
        }
    }

    int GetNearestNode()
    {
        int closestNode = 0;
        float storedLength = 1000;

        // Decide which way along the path we'll move
        pathDirection = 1;
        if (!waves.dontRandomizeDirection)
        {
            int pathDir = Random.Range(0, 2);
            switch (pathDir)
            {
                case (0):
                    pathDirection = -1;
                    break;
                case (1):
                    pathDirection = 1;
                    break;
            }
        }

        // Raycast to all nodes
        for (int i = 0; i < waves.enemyPath.Length; i++)
        {
            Vector2 dir = (waves.enemyPath[i].position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 50);
            if (hit && storedLength > Vector2.Distance(transform.position, waves.enemyPath[i].position))
            {
                closestNode = i;
                storedLength = Vector2.Distance(transform.position, waves.enemyPath[i].position);
            }
        }
        return closestNode;
    }

    // Returns index of next node on path
    int GetNextNode()
    {
        if ((currentNode == 0 && pathDirection == -1) || (currentNode == waves.enemyPath.Length - 1 && pathDirection == 1))
        {
            if (waves.useLoopPath)
            {
                if (currentNode == 0 && pathDirection == -1)
                    currentNode = waves.enemyPath.Length - 1;
                else
                    currentNode = 0;

                return currentNode;
            }
            else
                pathDirection *= -1;
        }

        return currentNode + pathDirection;
    }

    public float AngleBetween(Vector3 targetPosition)
    {
        Vector3 relative = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(-relative.x + Mathf.Sin(Time.deltaTime / 10) * 25, relative.y) * Mathf.Rad2Deg;
        return -angle;
    }

    // Searches for player in line of sight
    // If found, noticedPlayer is set to true
    public void RaycastForPlayer()
    {
        Vector2 dir = (ply.transform.position - transform.position).normalized;

        float scanDistance = stats.noticePlayerDistance;
        if (noticedPlayer)
            scanDistance = 25;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1, dir, scanDistance, playerScanIgnoreMask);
        if (hit)
        {
            Debug.DrawRay(transform.position, ((Vector3)hit.point - transform.position), Color.red, Time.deltaTime);
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

    public string GetCompassPointFromAngle(float angle)
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
        if (gm == null)
            yield break;

        gm.PlaySFXStoppable(gm.generalSfx[4], Random.Range(1.25f, 1.5f));
        spr.color = Color.red;
        float shakeAmount = 1;
        stats.health -= damage;

        if (stats.health <= 0 && !stats.overrideDeath)
            ExplodeIntoGoo();

        while (spr.color.g < 0.9f)
        {
            spr.color = Color.Lerp(spr.color, Color.white, 0.1f);
            if (!stats.overrideDeath)
                spr.transform.localPosition = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
            shakeAmount /= 1.5f;
            yield return new WaitForFixedUpdate();
        }
        if (!stats.overrideDeath)
            spr.transform.localPosition = Vector2.zero;
    }

    public void ReceiveShieldDamage()
    {
        gm.ShieldHit();
        StartCoroutine(ReceiveShieldDamageCoroutine());
    }

    IEnumerator ReceiveShieldDamageCoroutine()
    {
        if (stats.currentShieldValue > 0)
        {
            gm.PlaySFX(gm.playerSfx[1], Random.Range(0.9f, 1.1f));
            stats.currentShieldValue--;
        }
        if (stats.currentShieldValue > 1)
        {
            float shakeAmount = 0.5f;
            spr.color = Color.red;
            while (spr.color.g < 0.9f)
            {
                spr.color = Color.Lerp(spr.color, Color.white, 0.1f);
                spr.transform.localPosition = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
                shakeAmount /= 1.5f;
                yield return new WaitForFixedUpdate();
            }
            spr.transform.localPosition = Vector2.zero;
        }
        else if (stats.currentShieldValue == 0)
        {
            gm.ShieldBreak();
            ply.ReceiveDamage(25);
            ply.StunPlayer();
            ply.heldObject = null;
            ExplodeBig();
        }
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
        gm.PlaySFXStoppable(gm.generalSfx[0], Random.Range(0.85f, 1.15f));

        Vector2 gibSpawnPos = new Vector2(transform.position.x, transform.position.y - 0.5f + gibSpawnYOffset);
        for (int i = 0; i < stats.numGooDrops; i++)
        {
            Instantiate(gooDrops[Random.Range(0, gooDrops.Length)], gibSpawnPos, Quaternion.identity);
            Instantiate(splatterDrops[Random.Range(0, splatterDrops.Length)], gibSpawnPos, Quaternion.identity);
        }
        for(int i = 0; i < nonRandomDrops.Length; i++)
            Instantiate(nonRandomDrops[i], gibSpawnPos, Quaternion.identity);

        Die();
    }

    public void ExplodeBig()
    {
        print("Big explosion");
        gm.ScreenShake(10);
        Vector2 gibSpawnPos = new Vector2(transform.position.x, transform.position.y - 0.5f + gibSpawnYOffset);
        for (int i = 0; i < stats.numGooDrops; i++)
        {
            Instantiate(gooDrops[Random.Range(0, gooDrops.Length)], gibSpawnPos, Quaternion.identity);
            Instantiate(splatterDrops[Random.Range(0, splatterDrops.Length)], gibSpawnPos, Quaternion.identity);
        }
        Instantiate(enemyExplosion, transform.position, transform.rotation);
        Die();
    }

    void Die()
    {
        gm.IncreaseKills();
        Destroy(this.gameObject);
    }
}
