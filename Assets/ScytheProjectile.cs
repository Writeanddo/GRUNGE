using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheProjectile : MonoBehaviour
{
    public float radius;
    public int damage;

    bool canHitPlayer;
    bool hasHitPlayer;
    Vector3 focusPoint;
    Vector3 startPos;
    PlayerController ply;
    Transform crosshair;
    Transform sprite;
    SpriteRenderer spr;
    GameManager gm;
    List<Transform> hitEnemies;

    float timer;
    float alpha = 0;
    float tilt = 180;
    bool clearedEnemies;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.PlaySFX(gm.generalSfx[16]);
        sprite = transform.GetChild(0);
        spr = sprite.GetComponent<SpriteRenderer>();
        ply = FindObjectOfType<PlayerController>();
        crosshair = ply.transform.GetChild(1);
        focusPoint = (crosshair.position - transform.position).normalized * radius;
        startPos = transform.position + focusPoint;
        hitEnemies = new List<Transform>();

        tilt = AngleBetweenMouse(transform) - 90;
        print(tilt);

        StartCoroutine(InitializeDelay());
    }

    float AngleBetweenMouse(Transform reference)
    {
        Vector3 relative = reference.transform.InverseTransformPoint(crosshair.position);
        float angle = Mathf.Atan2(-relative.x, relative.y) * Mathf.Rad2Deg;
        return angle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (!hasHitPlayer)
        {
            if (timer < 0.6f)
            {
                // no clue
                transform.position = new Vector2(startPos.x + (radius * MCos(alpha) * MCos(tilt)) - (1f * MSin(alpha) * MSin(tilt)),
                                                 startPos.y + (radius * MCos(alpha) * MSin(tilt)) + (1f * MSin(alpha) * MCos(tilt)));
                alpha += 8f;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, ply.transform.position, Mathf.Clamp(Vector2.Distance(transform.position, ply.transform.position)/3, 0.05f, 1.5f));
            }

            if(alpha >= 180 && !clearedEnemies)
            {
                hitEnemies.Clear();
                clearedEnemies = true;
            }    

            sprite.Rotate(new Vector3(0, 0, -25));
        }

        if (timer > 0.82f && !hasHitPlayer)
        {
            EndSequence();
        }
    }

    float MCos(float value)
    {
        return Mathf.Cos(Mathf.Deg2Rad * value);
    }

    float MSin(float value)
    {
        return Mathf.Sin(Mathf.Deg2Rad * value);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasHitPlayer && collision.tag == "Enemy" && !hitEnemies.Contains(collision.transform))
        {
            hitEnemies.Add(collision.transform);
            collision.transform.GetComponent<EnemyScript>().ReceiveDamage(damage);
        }

        if (collision.tag == "Player" && canHitPlayer)
        {
            //EndSequence();
        }
    }

    void EndSequence()
    {
        print(timer);
        hasHitPlayer = true;
        ply.GetComponent<Rigidbody2D>().velocity = (ply.transform.position - transform.position).normalized * 10;
        gm.PlaySFX(gm.generalSfx[17]);
        StartCoroutine(DeathDelay());
    }

    IEnumerator InitializeDelay()
    {
        yield return new WaitForSeconds(0.25f);
        canHitPlayer = true;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0f);
        Destroy(this.gameObject);
    }
}
