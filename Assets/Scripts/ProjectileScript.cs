using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public GameObject explosion;
    public float speed = 32;
    Rigidbody2D rb;
    Transform crosshair;
    GameManager gm;
    CircleCollider2D c;
    bool hasMadeExplosion;
    bool delayFinished;

    private void Start()
    {
        c = GetComponent<CircleCollider2D>();
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right*speed;
        StartCoroutine(InitialWallDelay());
    }

    IEnumerator InitialWallDelay()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(FindObjectOfType<PlayerController>().transform.position, c.radius);
        for(int i = 0; i < cols.Length; i++)
        {
            if (cols[i].tag == "Enemy" || cols[i].tag == "Breakable")
            {
                if (cols[i].gameObject.layer != 8)
                {
                    if (!hasMadeExplosion)
                    {
                        hasMadeExplosion = true;
                        Instantiate(explosion, transform.position, Quaternion.identity);
                    }
                    Destroy(this.gameObject);
                }
            }
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        delayFinished = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!delayFinished)
            return;

        if (collision.tag == "Enemy" || collision.tag == "Wall" || collision.tag == "Breakable" || collision.tag == "SnotBubble")
        {
            if (collision.gameObject.layer != 8)
            {
                if (!hasMadeExplosion)
                {
                    hasMadeExplosion = true;
                    Instantiate(explosion, transform.position, Quaternion.identity);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
