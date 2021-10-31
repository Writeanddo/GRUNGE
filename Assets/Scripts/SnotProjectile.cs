using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnotProjectile : MonoBehaviour
{
    public float speed = 4;
    public int damage = 5;
    public float inaccuracyModifier = 0;
    public bool overrideDirection;

    [HideInInspector]
    public PlayerController ply;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public bool delayFinished;

    GameManager gm;
    Animator anim;
    bool popping;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        anim = GetComponentInChildren<Animator>();

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

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, ply.transform.position);
        rb.velocity += (Vector2)(ply.transform.position - transform.position).normalized * Mathf.Clamp(1 / (distance), 0, speed / 5)*0.65f; // the fuck
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
    }

    public IEnumerator InitialWallDelay()
    {
        yield return new WaitForSeconds(0.25f);
        delayFinished = true;
    }
    void OnTriggerEnter2D (Collider2D collision)
    {
        if (popping)
            return;

        if (collision.tag == "Player")
        {
            if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
            {
                ply.heldObject.GetComponent<EnemyScript>().ReceiveShieldDamage();
            }
            else
                ply.ReceiveDamage(damage);

            Pop(false);
        }
        if (collision.tag == "Wall" && delayFinished)
            Pop(true);
    }

    public void Pop(bool isSilent)
    {
        if (popping)
            return;

        if(!isSilent)
            gm.PlaySFX(gm.generalSfx[26]);
        popping = true;
        rb.velocity = Vector2.zero;
        anim.Play("SnotPop", -1, 0);
    }

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }
}
