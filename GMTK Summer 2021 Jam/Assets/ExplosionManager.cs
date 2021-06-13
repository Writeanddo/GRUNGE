using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public int damageMultiplier = 32;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3);
        gm.ScreenShake(6);

        foreach (Collider2D h in hits)
        {
            if (h.tag == "Enemy" || h.tag == "GrabbableEnemy")
            {
                Vector2 dir = (h.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(h.transform.position, transform.position);
                h.GetComponent<Rigidbody2D>().velocity = (dir * 25 / distance * 3);
                //print("Distance: " + distance + ", Damage: " + Mathf.Clamp(Mathf.RoundToInt(1 / distance * 25), 0, 32));
                h.GetComponent<EnemyScript>().ReceiveDamage(Mathf.Clamp(Mathf.RoundToInt(1 / distance * 25), 0, damageMultiplier));
            }
        }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
