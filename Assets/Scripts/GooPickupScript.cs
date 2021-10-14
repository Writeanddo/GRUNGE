using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooPickupScript : MonoBehaviour
{
    public float pickupDistance = 6;
    public int gooAmount;

    float stuckInWallTimer;
    Rigidbody2D rb;
    PlayerController ply;
    GameManager gm;
    bool playedSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ply = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();

        rb.velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10) * 2);
        rb.angularVelocity = Random.Range(-100, 100);

        CheckIfStuck();
    }

    void CheckIfStuck()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].tag == "Wall")
            {
                Vector3 dir = ((Vector2)transform.position - cols[i].ClosestPoint(transform.position)).normalized;
                transform.position = (Vector3)cols[i].ClosestPoint(transform.position) + dir;
                rb.velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10) * 2);
                print("Got unstuck");
                break;
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, Vector2.zero, 0.05f);

        // Move towards player if we're withing pickup range
        float distance = Vector2.Distance(transform.position, ply.transform.position);
        if (distance <= pickupDistance)
        {
            rb.velocity += (Vector2)(ply.transform.position - transform.position).normalized * Mathf.Clamp(1 / (distance / 4), -10, 10);
        }

        rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0, 0.05f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!playedSound)
            {
                gm.PlayGooSFX(gm.gooPickupSounds[Random.Range(0, 3)]);
                playedSound = true;
            }

            ply.stats.goo = Mathf.Clamp(ply.stats.goo + gooAmount, 0, ply.stats.maxGoo);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            
            stuckInWallTimer += Time.deltaTime;
            if (stuckInWallTimer >= 2)
                Destroy(this.gameObject);
        }
    }
}
