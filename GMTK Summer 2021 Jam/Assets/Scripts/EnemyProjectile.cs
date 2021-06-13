using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 4;
    public int damage = 5;
    public float inaccuracyModifier = 0;
    public bool overrideDirection;

    PlayerController ply;
    Rigidbody2D rb;
    bool delayFinished;
    // Start is called before the first frame update
    void Start()
    {
        ply = FindObjectOfType<PlayerController>();
        if (!overrideDirection)
            Launch((ply.transform.position - transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * inaccuracyModifier).normalized * speed);

        StartCoroutine(InitialWallDelay());
    }

    public void Launch(Vector2 vel)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = vel;
    }

    IEnumerator InitialWallDelay()
    {
        yield return new WaitForSeconds(0.25f);
        delayFinished = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
            {
                ply.heldObject.GetComponent<EnemyScript>().ReceiveShieldDamage();
            }
            else
                ply.ReceiveDamage(damage);
            Destroy(this.gameObject);
        }
        if (collision.tag == "Wall" && delayFinished)
            Destroy(this.gameObject);
    }
}
