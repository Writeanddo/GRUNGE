using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool canBreak;
    public int hitsBeforeBreak;
    public int baseDamageUponHitting = 5;
    public bool isHeld;
    [HideInInspector]
    public Rigidbody2D rb;
    Collider2D col;

    int hitsTaken;
    int defaultLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        defaultLayer = gameObject.layer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHeld)
        {
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
        if (rb.velocity.magnitude > 3)
        {
            if(canBreak)
            {
                hitsTaken++;
                if (hitsTaken >= hitsBeforeBreak)
                    Destroy(this.gameObject);
            }
            if (collision.tag == "Enemy" || collision.tag == "Enemy")
            {
                EnemyScript e = collision.GetComponent<EnemyScript>();
                e.GetComponent<Rigidbody2D>().velocity = (e.transform.position - transform.position).normalized * rb.velocity.magnitude/1.5f;
                e.ReceiveDamage(Mathf.RoundToInt(baseDamageUponHitting * Mathf.Clamp(rb.velocity.magnitude/5, 1, 2)));
            }
        }
    }
}
