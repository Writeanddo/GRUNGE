using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    PlayerController ply;
    GameManager gm;
    Grabbable g;
    // Start is called before the first frame update
    void Start()
    {
        g = GetComponent<Grabbable>();
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
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

    void UseItem()
    {
        gm.PlayPrioritySFX(gm.generalSfx[12]);
        ply.stats.health = ply.stats.maxHealth;
        ply.stats.goo = ply.stats.maxGoo;
        ply.heldObject = null;
        Destroy(this.gameObject);
    }
}
