using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool canBeGrabbed = true;
    public bool isProp;
    public bool canBreak;
    public bool breakFromPropContact;
    public bool breakFromCharacterContact;
    public bool rotatePropFragments;
    public int hitsBeforeBreak;
    public int baseDamageUponHitting = 5;
    public Vector2 objectOffsetWhenHeld; // How high the hand should appear above this object
    public Vector2 handOffsetWhenHeld;  // How high the hand should appear above the player when this object is held
    public bool isHeld;
    public bool isBeingThrown;
    public Sprite damagedSprite;
    public Sprite[] propFragments; // Sprites to apply to propGib
    public GameObject propGib;
    public AudioClip breakSound;

    bool hasExploded;
    [HideInInspector]
    public Rigidbody2D rb;
    Collider2D col;
    EnemyScript thisEnemy;
    SpriteRenderer spr;
    GameManager gm;

    List<Transform> hitEnemies;

    public int hitsTaken;
    bool breaking;
    int defaultLayer;

    void Start()
    {
        if (!isProp)
            thisEnemy = GetComponent<EnemyScript>();
        else
            spr = GetComponent<SpriteRenderer>();

        hitEnemies = new List<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        gm = FindObjectOfType<GameManager>();
        defaultLayer = gameObject.layer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHeld)
        {
            if (hitEnemies.Count > 0)
                hitEnemies.Clear();

            if (isProp)
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.15f);
        }
    }

    public void SetHeldState(bool state)
    {
        isHeld = state;
        rb.isKinematic = state;

        if (!state)
        {
            StartCoroutine(ColliderEnableDelay());
        }
        else
            gameObject.layer = 8;
    }

    IEnumerator ColliderEnableDelay()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        gameObject.layer = defaultLayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deal damage if we hit an enemy at a fast enough speed, break apart if we get hit too much
        if (isBeingThrown && rb.velocity.magnitude > 3)
        {
            // Change damage based on how many hits we've taken
            float damageMultiplier = 1;
            if (!isProp && (collision.tag == "Enemy" || collision.tag == "Wall"))
            {
                switch (thisEnemy.stats.currentShieldValue)
                {
                    case (3):
                        damageMultiplier = 1.5f;
                        break;
                    case (2):
                        damageMultiplier = 2;
                        break;
                    case (1):
                        if (!hasExploded)
                        {
                            hasExploded = true;
                            thisEnemy.ExplodeBig();
                        }
                        break;
                }
            }

            if (collision.tag == "Enemy" || collision.tag == "Wall" && !hitEnemies.Contains(collision.transform))
            {
                if (canBreak)
                {
                    TakeDamage(collision.gameObject);
                }

                hitEnemies.Add(collision.transform);
                EnemyScript e = collision.GetComponent<EnemyScript>();
                if (e != null && e.name != "Boss")
                {
                    e.GetComponent<Rigidbody2D>().velocity = (e.transform.position - transform.position).normalized * rb.velocity.magnitude / 1.25f;
                    int damage = Mathf.RoundToInt(baseDamageUponHitting * Mathf.Clamp(rb.velocity.magnitude / 5, 1, 2.5f) * damageMultiplier);
                    e.ReceiveDamage(damage);
                }
            }

            // Damage self if we get slammed into a concrete wall at mach eleven
            if (collision.tag == "Wall" && !isProp)
            {
                int damage = Mathf.RoundToInt(baseDamageUponHitting * Mathf.Clamp(rb.velocity.magnitude / 5, 1, 2.5f) * damageMultiplier);
                thisEnemy.ReceiveDamage(Mathf.RoundToInt(thisEnemy.stats.maxHealth / 3f + 1));
            }
        }

        // If this is a static prop, check if we're getting hit by a thrown object
        if (breakFromPropContact && collision.tag == "Grabbable")
        {
            Grabbable g = collision.GetComponent<Grabbable>();
            if (g.isBeingThrown)
            {
                if (canBreak)
                {
                    TakeDamage(collision.gameObject);
                }
            }
        }

        // Also check to see if we should take damage when something hits us (player / enemy)
        if (breakFromCharacterContact && (collision.tag == "Enemy" || collision.tag == "Player"))
            if (canBreak)
            {
                print("Making me killing you");
                TakeDamage(collision.gameObject);
            }
    }

    public void TakeDamage(GameObject g)
    {
        hitsTaken++;
        if (hitsTaken > hitsBeforeBreak / 2 && damagedSprite != null)
            spr.sprite = damagedSprite;

        if (hitsTaken >= hitsBeforeBreak && !breaking)
        {
            breaking = true;
            if (breakSound != null)
                gm.PlaySFX(breakSound);

            if (g.tag == "Grabbable")
            {
                Rigidbody2D grb = g.GetComponent<Rigidbody2D>();
                grb.velocity = -grb.velocity;
            }

            foreach (Sprite s in propFragments)
            {
                GameObject go = Instantiate(propGib, transform.position, transform.rotation);
                go.GetComponent<SpriteRenderer>().sprite = s;
                if (rotatePropFragments)
                    go.GetComponent<GibScript>().rotate = true;
            }
            Destroy(this.gameObject);
        }
    }
}
