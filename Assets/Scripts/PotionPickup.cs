using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    PlayerController ply;
    GameManager gm;
    Grabbable g;
    SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        g = GetComponent<Grabbable>();
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();

        StartCoroutine(DestroyAfterTime());
    }

    private void FixedUpdate()
    {
        if (g.isHeld && Vector3.Distance(transform.position, ply.transform.position) < 1.5f)
            UseItem();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UseItem();
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(15);
        for (int i = 0; i < 25; i++)
        {
            spr.color = new Color(1, 1, 1, 0.35f);
            yield return new WaitForSeconds(0.1f);
            spr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
    }

    void UseItem()
    {
        gm.PlayPrioritySFX(gm.generalSfx[12]);
        ply.stats.health = ply.stats.maxHealth;
        ply.stats.goo = ply.stats.maxGoo;

        if(ply.heldObject == this.gameObject)
            ply.heldObject = null;

        Destroy(this.gameObject);
    }
}
