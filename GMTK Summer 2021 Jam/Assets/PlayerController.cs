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
    }

    public PlayerStats stats;
    public GameObject projectile;

    public bool handLaunched;
    public bool canLaunchHand = true;
    public Transform heldObject;

    Transform crosshair;
    Transform gun;
    Transform gunTargetPos;
    Transform handTargetPos;
    Transform handHolder;
    Transform hand;
    Animator anim;
    GameManager gm;
    Rigidbody2D rb;
    HandGrabManager hgm;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        crosshair = transform.GetChild(1);
        gunTargetPos = anim.transform.GetChild(0);
        gun = anim.transform.GetChild(1);
        handTargetPos = anim.transform.GetChild(2);
        handHolder = anim.transform.GetChild(3);
        hand = anim.transform.GetChild(3).GetChild(0);

        hgm = FindObjectOfType<HandGrabManager>();
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        UpdateInputAxes();
        UpdateMovementAnimations();
        gun.transform.position = Vector2.Lerp(gun.transform.position, gunTargetPos.position, 0.25f);
        if (!handLaunched)
            handHolder.transform.position = Vector2.Lerp(handHolder.transform.position, handTargetPos.position, 0.25f);
    }

    void Update()
    {
        UpdateInputButtons();
    }

    void UpdateInputAxes()
    {
        // Movement
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 speed = new Vector2(horiz, vert);
        rb.velocity = Vector3.ClampMagnitude(speed, 1) * stats.maxSpeed;
        stats.speed = rb.velocity.magnitude;
    }

    void UpdateMovementAnimations()
    {
        if (rb.velocity.magnitude < 0.5f)
            CheckAndPlayClip("Player_Face" + GetCompassDirection(AngleBetweenMouse()));
        else
        {
            anim.SetFloat("WalkSpeed", rb.velocity.magnitude / 4);
            CheckAndPlayClip("Player_Walk" + GetCompassDirection(AngleBetweenMouse()));
        }

        // Grapple hand animations
        string animName = "";
        if (heldObject != null)
        {
            animName = "Hand_HoldN";
        }
        else if (!handLaunched)
        {
            animName = "Hand_Face" + GetCompassDirectionFourDirectional(AngleBetweenMouse());
        }
        hgm.CheckAndPlayClip(animName);

    }

    void UpdateInputButtons()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireGun();
        }
        if (Input.GetButtonDown("Fire2") && canLaunchHand)
        {
            if (!handLaunched)
            {
                Vector2 dir = (crosshair.position - handTargetPos.position).normalized;
                if (heldObject == null)
                {
                    // launch hand
                    handLaunched = true;
                    canLaunchHand = false;
                    StartCoroutine(WaitForHandHit((Vector2)handTargetPos.position + dir * 15));
                }
                else
                {
                    // throw held object
                    hgm.ThrowItem(dir * 75);
                    handHolder.transform.position += (Vector3)dir;
                    canLaunchHand = false;
                    StartCoroutine(HandCooldown());
                }
            }
        }
    }

    IEnumerator WaitForHandHit(Vector3 targetPos)
    {
        hgm.CheckAndPlayClip("Hand_Grab" + GetCompassDirectionFourDirectional(AngleBetweenMouse()));

        Vector3 storedPos = handHolder.transform.position;
        while (handLaunched && Vector2.Distance(transform.position, handHolder.transform.position) < 8 && Vector2.Distance(handHolder.transform.position, targetPos) > 0.25f)
        {
            handHolder.transform.position = storedPos;
            handHolder.transform.position = Vector3.Lerp(handHolder.transform.position, Vector2.MoveTowards(handHolder.transform.position, targetPos, 1f), 0.35f);
            storedPos = handHolder.transform.position;
            yield return new WaitForFixedUpdate();
        }
        handLaunched = false;

        while (Vector2.Distance(handHolder.transform.position, handTargetPos.position) > 0.25f)
            yield return null;

        //yield return new WaitForSeconds(0.25f);
        canLaunchHand = true;
    }

    IEnumerator HandCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canLaunchHand = true;
    }

    float AngleBetweenMouse()
    {
        Vector3 relative = transform.InverseTransformPoint(crosshair.position);
        float angle = Mathf.Atan2(-relative.x, relative.y) * Mathf.Rad2Deg;
        return -angle;
    }

    void FireGun()
    {
        gm.ScreenShake(5);
        Vector3 offset = (crosshair.transform.position - transform.position).normalized;
        gun.transform.position -= offset;
        Instantiate(projectile, gunTargetPos.position, Quaternion.Euler(0, 0, -AngleBetweenMouse() + 90));
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
}
