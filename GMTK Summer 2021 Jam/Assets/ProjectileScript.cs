using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public GameObject explosion;
    Rigidbody2D rb;
    Transform crosshair;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right*32;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Wall")
        {
            if (collision.gameObject.layer != 8)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}
