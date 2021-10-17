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
    bool hasMadeExplosion;
    bool delayFinished;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right*speed;
        StartCoroutine(InitialWallDelay());
    }

    IEnumerator InitialWallDelay()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        delayFinished = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!delayFinished)
            return;

        if (collision.tag == "Enemy" || collision.tag == "Wall" || collision.tag == "Breakable")
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
