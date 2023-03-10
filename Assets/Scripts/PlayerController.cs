using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int health;
        public int maxHealth;
        public int goo;
        public int maxGoo;
        public float speed;
        public float maxSpeed;
        public int shotGooUsage = 15;
        public int currentWeapon = 0; // 0 = gun, 1 = shotgun, 2 = tommy gun, 3 = scythe
    }

    public PlayerStats stats;
    public GameObject[] projectiles;

    public bool canMove;
    public bool isDying;
    public bool canShoot = true;
    public bool reloading;
    public bool siphoningHealth;
    public bool handLaunched;
    public bool canLaunchHand = true;
    public Transform heldObject;

    [HideInInspector]
    public Vector2 additionalForce;

    float speedMultiplier = 1;
    float storedMaxSpeed;
    bool playedDieSequence;
    public bool chargingAttack;
    public bool chargeReady;
    public bool rightClickReleased;
    bool throwingScythe;

    Vector2 gizmoOffset;

    Transform crosshair;
    Transform gun;
    Transform gunTargetPos;
    Transform gunTwoHandedTargetPos;
    Transform handTargetPos;
    Transform handHolder;
    Transform handShaker;
    Transform hand;
    Animator anim;
    Animator gunAnim;
    Transform gunTwoHandedHolder;
    Animator gunTwoHandedAnim;
    Animator glintAnim;
    SpriteRenderer spr;
    GameManager gm;
    Rigidbody2D rb;
    HandGrabManager hgm;

    Sprite[] currentWeaponFrames;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        spr = anim.GetComponent<SpriteRenderer>();
        crosshair = transform.GetChild(1);
        gunTargetPos = anim.transform.GetChild(0);
        gun = anim.transform.GetChild(1);
        gunAnim = gun.GetComponent<Animator>();
        handTargetPos = anim.transform.GetChild(2);
        handHolder = anim.transform.GetChild(3);
        handShaker = anim.transform.GetChild(3).GetChild(0);
        hand = anim.transform.GetChild(3).GetChild(0).GetChild(0);
        gunTwoHandedTargetPos = anim.transform.GetChild(4);
        gunTwoHandedHolder = anim.transform.GetChild(5);
        gunTwoHandedAnim = gunTwoHandedHolder.GetChild(0).GetComponent<Animator>();
        glintAnim = transform.GetChild(4).GetComponent<Animator>();

        hgm = FindObjectOfType<HandGrabManager>();
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        storedMaxSpeed = stats.maxSpeed;

        StartCoroutine(GooRegen());
    }

    private void FixedUpdate()
    {
        if (stats.health <= 0 && !playedDieSequence)
            StartCoroutine(Die());

        if (canMove && !isDying && stats.health > 0)
        {
            UpdateInputAxes();
            UpdateMovementAnimations();
        }

        gun.transform.position = Vector2.Lerp(gun.transform.position, gunTargetPos.position, 0.25f);
        gunTwoHandedHolder.transform.position = Vector2.Lerp(gunTwoHandedHolder.transform.position, gunTwoHandedTargetPos.position, 0.25f);
        gunTwoHandedHolder.transform.localRotation = Quaternion.Lerp(gunTwoHandedHolder.transform.localRotation, Quaternion.identity, 0.25f);

        if (!handLaunched)
        {
            //Vector2.MoveTowards(handHolder.transform.position, handTargetPos.position, );
            handHolder.transform.position = Vector2.Lerp(handHolder.transform.position, handTargetPos.position, 0.25f);
        }
    }

    void Update()
    {
        if (gm.paused || !canMove || isDying || stats.health <= 0)
            return;

        UpdateInputButtons();
    }

    void UpdateInputAxes()
    {
        // Movement
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 speed = new Vector2(horiz, vert);

        float damping = 0.5f;

        if (isDying || stats.health <= 0)
            additionalForce = Vector2.zero;

        if (additionalForce == Vector2.zero && rb.velocity.magnitude > stats.maxSpeed)
        {
            rb.velocity -= rb.velocity * 0.1f;
        }
        else
        {
            rb.velocity = (Vector3.ClampMagnitude(speed, 1) * stats.maxSpeed * speedMultiplier) + (Vector3)additionalForce;
            stats.speed = rb.velocity.magnitude;
        }

        additionalForce = Vector2.Lerp(additionalForce, Vector2.zero, 0.1f);
    }

    void UpdateMovementAnimations()
    {
        string dir = GetCompassDirection(AngleBetweenMouse(transform));
        if (rb.velocity.magnitude < 0.5f)
            CheckAndPlayClip("Player_Face" + dir);
        else
        {
            anim.SetFloat("WalkSpeed", rb.velocity.magnitude / 4);
            CheckAndPlayClip("Player_Walk" + dir);
        }

        // Update weapon sprite
        string gunPrefix = "";
        switch (stats.currentWeapon)
        {
            case (0):
                gunPrefix = "Gun";
                break;
            case (1):
                gunPrefix = "Shotgun";
                break;
            case (2):
                gunPrefix = "MG";
                break;
            case (10):
                gunPrefix = "Scythe";
                break;
            case (11):
                gunPrefix = "Knuckles";
                break;
        }

        // If current weapon index is 10 or greater, we're using a two-handed weapon
        if (stats.currentWeapon >= 10)
        {
            stats.shotGooUsage = 0;
            hgm.CheckAndPlayClip("Blank");
            CheckAndPlayClip("Blank", gunAnim);
            if (!throwingScythe)
                CheckAndPlayClip(gunPrefix + "_" + dir, gunTwoHandedAnim);
            else
                CheckAndPlayClip("Blank", gunTwoHandedAnim);
        }
        else
        {
            CheckAndPlayClip("Blank", gunTwoHandedAnim);
            CheckAndPlayClip(gunPrefix + "_" + dir, gunAnim);

            // Grapple hand animations
            string animName = "";
            if (heldObject != null)
            {
                animName = "Hand_HoldN";
            }
            else if (!handLaunched)
            {
                animName = "Hand_Face" + GetCompassDirectionFourDirectional(AngleBetweenMouse(transform));
            }
            hgm.CheckAndPlayClip(animName);
        }
    }

    void UpdateInputButtons()
    {
        if (gm.paused || isDying || stats.health <= 0)
            return;

        // Shoot gun
        if (Input.GetButton("Fire1"))
        {
            if (canShoot && !reloading && stats.goo >= stats.shotGooUsage)
            {
                gm.firedGun = true;
                FireGun();
            }
        }

        // Reload sound
        if (canShoot && Input.GetButtonDown("Fire1") && !reloading && (stats.currentWeapon != 11 && !chargingAttack))
        {
            Vector3 offset = (crosshair.transform.position - gunTargetPos.position).normalized;
            gun.transform.position -= offset;
            gm.PlaySFX(gm.playerSfx[10]);
        }

        // Shoot hand / perform secondary weapon action
        if (Input.GetButtonDown("Fire2"))
        {
            // Hand launch
            if (stats.currentWeapon < 10)
            {
                // If we aren't holding an object, launch hand
                if (heldObject == null && canLaunchHand)
                {
                    if (!handLaunched)
                    {
                        Vector2 dir = (crosshair.position - handTargetPos.position).normalized;

                        // launch hand
                        //gm.PlaySFX(gm.playerSfx[4], 0.9f);
                        handLaunched = true;
                        canLaunchHand = false;
                        rightClickReleased = false;
                        StartCoroutine(WaitForHandHit((Vector2)handTargetPos.position + dir * 15));
                    }
                }
            }
        }

        // Charge attack checks - needs to be here in case we rapid clicked right mouse when hand was out
        if (Input.GetButton("Fire2"))
        {
            // If we're holding an object and its an enemy, charge it until we release rmb
            if (!chargingAttack && heldObject != null && rightClickReleased && canLaunchHand)
            {
                if (heldObject.tag == "Enemy")
                {
                    StartCoroutine(ChargeHeldEnemy());
                }
            }
            // Scythe procedure
            if (stats.currentWeapon == 10 && !reloading && !chargingAttack)
            {
                // temp?
                //stats.maxSpeed = storedMaxSpeed * 0.75f;
                StartCoroutine(PrepareChargeAttack());
            }
        }

        // Throw held item / Release charge
        if (Input.GetButtonUp("Fire2"))
        {
            // Have to release RMB once before being able to charge again (to prevent immediate charge upon grab)
            if (!rightClickReleased && (handLaunched || heldObject != null))
                rightClickReleased = true;

            else if (heldObject != null && rightClickReleased && canLaunchHand)
            {
                gm.StopPrioritySFX();

                // Throw held object
                Vector2 dir = (crosshair.position - handTargetPos.position).normalized;

                gm.PlaySFX(gm.playerSfx[2]);
                hgm.ThrowItem(dir * 75);
                handHolder.transform.position += (Vector3)dir;
                canLaunchHand = false;
                StartCoroutine(HandAfterThrowingCooldown());
                rightClickReleased = false;
                chargingAttack = false;

                stats.maxSpeed = storedMaxSpeed;

            }
            if (stats.currentWeapon == 10 && !reloading)
            {
                if (chargingAttack && chargeReady)
                {
                    gm.PlaySFX(gm.playerSfx[2]);
                    throwingScythe = true;
                    reloading = true;
                    Instantiate(projectiles[2], transform.position, Quaternion.identity);
                    StartCoroutine(WaitForGunReload(0.82f));
                }

                chargingAttack = false;
                stats.maxSpeed = storedMaxSpeed;
            }

            chargeReady = false;
        }

        // Convert goo into health
        if ((Input.GetKeyDown(KeyCode.E)|| Input.GetKeyDown(KeyCode.RightControl)) && stats.health < stats.maxHealth && stats.goo >= stats.maxGoo * 0.5f && !siphoningHealth)
        {
            siphoningHealth = true;
            StartCoroutine(ConvertGooToHealth());
        }
    }

    public void ForceRightClickRelease()
    {
        rightClickReleased = true;
    }

    IEnumerator PrepareChargeAttack()
    {
        chargingAttack = true;
        float timer = 0;
        gm.PlaySFXStoppablePriority(gm.playerSfx[11], 1);

        while (timer < 0.5f && chargingAttack)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (chargingAttack)
        {
            gm.PlaySFX(gm.playerSfx[4]);
            glintAnim.Play("ChargeGlint", -1, 0);
            chargeReady = true;
        }

        gm.StopPrioritySFX();
    }

    IEnumerator ChargeHeldEnemy()
    {
        EnemyScript e = heldObject.GetComponent<EnemyScript>();
        chargingAttack = true;
        float timer = 0;
        //yield return new WaitForSeconds(0.1f);
        //stats.maxSpeed = storedMaxSpeed * 0.5f;
        while (handLaunched)
            yield return null;

        while (e.stats.currentShieldValue > 1 && chargingAttack)
        {
            timer = 0;
            float multiplier = 3 - (e.stats.currentShieldValue * 0.5f);

            gm.PlaySFXStoppablePriority(gm.playerSfx[12], multiplier / 2f);
            while (timer < 0.8f && chargingAttack)
            {
                timer += Time.fixedDeltaTime;
                handShaker.transform.localPosition = new Vector2(0, 0.33f * multiplier * Mathf.Sin(timer * 8 * Mathf.PI * multiplier));
                yield return new WaitForFixedUpdate();
            }

            if (chargingAttack && e.stats.currentShieldValue > 1)
                e.ReceiveShieldDamage();
        }

        handShaker.transform.localPosition = Vector2.zero;
        stats.maxSpeed = storedMaxSpeed;
        //if (e.stats.currentShieldValue > 1 && heldObject == e.gameObject)

    }

    IEnumerator GooRegen()
    {
        EnemyWaveManager ewm = FindObjectOfType<EnemyWaveManager>();

        while (!ewm.isSpawningEnemies)
            yield return null;

        while (ewm.isSpawningEnemies)
        {
            yield return new WaitForSeconds(1);
            while (gm.paused || isDying)
                yield return null;

            stats.goo += 2;
        }
    }

    public void SetVisible()
    {
        spr.color = Color.white;
        gunAnim.GetComponent<SpriteRenderer>().color = Color.white;
        hgm.GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator Die()
    {
        gm.canPause = false;
        playedDieSequence = true;
        chargingAttack = false;
        chargeReady = false;
        isDying = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        gm.StopMusic();
        gm.gameOver = true;
        FindObjectOfType<CrosshairController>().SetVisible(false);
        CheckAndPlayClip("Player_Die");
        yield return new WaitForSeconds(2);
        if (heldObject != null)
            hgm.DropItem();
        isDying = false;
        yield return new WaitForSeconds(1);
        gm.StartCoroutine(gm.GameOverSequence());
    }

    IEnumerator ConvertGooToHealth()
    {
        gm.PlaySFX(gm.generalSfx[12]);
        stats.health += Mathf.RoundToInt(stats.maxHealth * 0.28f);
        stats.goo -= Mathf.RoundToInt(stats.maxGoo * 0.5f);
        yield return new WaitForSeconds(0.5f);
        siphoningHealth = false;
    }

    IEnumerator WaitForGunReload(float time)
    {
        yield return new WaitForSeconds(time);
        reloading = false;
        throwingScythe = false;
    }

    IEnumerator WaitForHandHit(Vector3 targetPos)
    {
        hgm.CheckAndPlayClip("Hand_Grab" + GetCompassDirectionFourDirectional(AngleBetweenMouse(hand.transform)));

        // Perform frame 1 scan for enemies that might've already been in our collider
        hgm.InitialGrabbableScan();

        Vector3 storedPos = handHolder.transform.position;
        while (handLaunched && Vector2.Distance(transform.position, handHolder.transform.position) < 6 && Vector2.Distance(handHolder.transform.position, targetPos) > 0.25f)
        {
            handHolder.transform.position = storedPos;
            handHolder.transform.position = Vector3.Lerp(handHolder.transform.position, Vector2.MoveTowards(handHolder.transform.position, targetPos, 1f), 0.45f);
            storedPos = handHolder.transform.position;

            if (isDying)
                yield break;
            yield return new WaitForFixedUpdate();
        }
        handLaunched = false;

        while (Vector2.Distance(handHolder.transform.position, handTargetPos.position) > 0.25f)
            yield return null;

        if (heldObject != null)
        {
            print("Hit enemy, waiting");
            StartCoroutine(HandReturnToBodyCooldown());
        }
        else
            canLaunchHand = true;
    }

    public void ReceiveDamage(int damage)
    {
        if (stats.health > 0)
            StartCoroutine(ReceiveDamageCoroutine(damage));
    }

    IEnumerator ReceiveDamageCoroutine(int damage)
    {
        gm.PlaySFX(gm.playerSfx[0]);
        spr.color = Color.red;
        float shakeAmount = 0.5f;
        stats.health -= damage;

        while (spr.color.g < 0.9f)
        {
            spr.color = Color.Lerp(spr.color, Color.white, 0.1f);
            spr.transform.localPosition = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
            shakeAmount /= 1.5f;
            yield return new WaitForFixedUpdate();
        }
        spr.transform.localPosition = Vector2.zero;
    }

    IEnumerator HandAfterThrowingCooldown()
    {
        yield return new WaitForSeconds(0.15f);
        canLaunchHand = true;
    }

    IEnumerator HandReturnToBodyCooldown()
    {
        yield return new WaitForSeconds(0.05f);
        canLaunchHand = true;
    }

    public void StunPlayer()
    {
        canLaunchHand = false;

        speedMultiplier = 0.625f;
        StartCoroutine(WaitForStunCompletion());
    }

    IEnumerator WaitForStunCompletion()
    {
        yield return new WaitForSeconds(5);
        canLaunchHand = true;
        speedMultiplier = 1;
    }

    float AngleBetweenMouse(Transform reference)
    {
        Vector3 relative = reference.transform.InverseTransformPoint(crosshair.position);
        float angle = Mathf.Atan2(-relative.x, relative.y) * Mathf.Rad2Deg;
        return -angle;
    }

    public void Freeze()
    {
        string dir = GetCompassDirection(AngleBetweenMouse(transform));
        CheckAndPlayClip("Player_Face" + dir);
        rb.velocity = Vector2.zero;
        canMove = false;
    }

    void FireGun()
    {
        if (stats.currentWeapon == 0)
        {
            stats.shotGooUsage = 22;
            gm.PlaySFX(gm.playerSfx[5]);
            gm.ScreenShake(3f);
            Vector3 offset = (crosshair.transform.position - gunTargetPos.position).normalized;
            gun.transform.position -= offset;
            Instantiate(projectiles[0], gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse(gun.transform) + 90));
            reloading = true;

            stats.goo -= stats.shotGooUsage;

            StartCoroutine(WaitForGunReload(0.3f));
        }
        else if (stats.currentWeapon == 1)
        {
            stats.shotGooUsage = 20;
            gm.PlaySFX(gm.playerSfx[8]);
            gm.ScreenShake(3f);
            Vector3 offset = (crosshair.transform.position - gunTargetPos.position).normalized;
            gun.transform.position -= offset;
            Instantiate(projectiles[3], gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse(gun.transform) + 80));
            Instantiate(projectiles[3], gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse(gun.transform) + 90));
            Instantiate(projectiles[3], gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse(gun.transform) + 100));
            reloading = true;

            stats.goo -= stats.shotGooUsage;

            StartCoroutine(WaitForGunReload(1.25f));
        }
        else if (stats.currentWeapon == 2)
        {
            stats.shotGooUsage = 2;
            gm.PlaySFX(gm.playerSfx[9]);
            Vector3 offset = (crosshair.transform.position - gunTargetPos.position).normalized;
            gun.transform.position -= offset;
            Instantiate(projectiles[1], gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse(gun.transform) + 90));
            reloading = true;

            stats.goo -= stats.shotGooUsage;

            StartCoroutine(WaitForGunReload(0.1f));
        }
        else if (stats.currentWeapon == 10)
        {
            stats.shotGooUsage = 0;
            reloading = true;
            Vector3 offset = (crosshair.transform.position - gunTargetPos.position).normalized;
            gunTwoHandedHolder.transform.position += offset * 3;
            gunTwoHandedHolder.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, gunTwoHandedHolder.transform.localRotation.z - 180));
            additionalForce = offset * 12;

            Instantiate(projectiles[4], transform.position + offset * 3, Quaternion.LookRotation(transform.forward, offset));
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + offset * 3, 2.75f);
            gizmoOffset = offset * 3;

            bool hitEnemy = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].tag == "Enemy")
                {
                    hits[i].GetComponent<EnemyScript>().ReceiveDamage(30);
                    hitEnemy = true;
                }
                if (hits[i].tag == "SnotBubble")
                {
                    hits[i].SendMessage("Pop", false);
                    hitEnemy = true;
                }
                if(hits[i].tag == "EnemyProjectile")
                {
                    Destroy(hits[i].gameObject);
                    hitEnemy = true;
                }
                if(hits[i].tag == "Breakable" || hits[i].tag == "BreakableIgnoreProjectiles")
                {
                    hits[i].GetComponent<Grabbable>().TakeDamage(this.gameObject, 30);
                }
            }
            if (!hitEnemy)
                gm.PlaySFX(gm.playerSfx[14]);
            else
                gm.PlaySFX(gm.playerSfx[13]);

            StartCoroutine(WaitForGunReload(0.6f));
        }
    }

    private void OnDrawGizmos()
    {
        if (gizmoOffset != Vector2.zero)
            Gizmos.DrawWireSphere(transform.position + (Vector3)gizmoOffset, 2.75f);
    }

    string GetCompassDirection(float angle)
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

    string GetCompassDirectionFourDirectional(float angle)
    {
        // -90 = W, 0 = N, 90 = E, 180/-180 = S

        if (angle >= -180 && angle < -135)
            return "S";
        else if (angle >= -135 && angle < -45)
            return "W";
        else if (angle >= -45 && angle < 45)
            return "N";
        else if (angle >= 45 && angle < 135)
            return "E";
        else
            return "S";
    }

    public void CheckAndPlayClip(string clipName)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }

    public void CheckAndPlayClip(string clipName, Animator a)
    {
        if (!a.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            a.Play(clipName);
        }
    }
}
