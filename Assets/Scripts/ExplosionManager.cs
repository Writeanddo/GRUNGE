using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public int damageMultiplier = 32;
    public AudioClip explosionSound;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        gm.PlaySFX(explosionSound);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3);
        gm.ScreenShake(6);

        foreach (Collider2D h in hits)
        {
            if (h.tag == "Enemy" && h.transform.parent == null)
            {
                Vector2 dir = (h.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(h.transform.position, transform.position);
                //print("Distance: " + distance + ", Damage: " + Mathf.Clamp(Mathf.RoundToInt(1 / distance * 25), 0, 32));
                EnemyScript e = h.GetComponent<EnemyScript>();
                if (!e.stats.overrideDeath)
                    h.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(dir * 25 / distance * 3, 25);
                int damage = Mathf.Clamp(Mathf.RoundToInt(1 / distance * 25), 0, damageMultiplier);
                h.GetComponent<EnemyScript>().ReceiveDamage(damage);
            }
        }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
