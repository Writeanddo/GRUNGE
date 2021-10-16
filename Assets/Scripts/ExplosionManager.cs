using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    
    public int damage = 27;
    public float knockbackMultiplier = 1;
    public AudioClip explosionSound;
    public GameObject[] gibs;

    List<string> hitObjectNames;
    GameManager gm;


    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        hitObjectNames = new List<string>();
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        if(explosionSound != null)
            gm.PlaySFXStoppablePriority(explosionSound, 1);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3);
        gm.ScreenShake(6*knockbackMultiplier);

        foreach (Collider2D h in hits)
        {
            if (!hitObjectNames.Contains(h.transform.name) && h.tag == "Enemy" && h.transform.parent == null)
            {
                Vector2 dir = (h.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(h.transform.position, transform.position);
                //print("Distance: " + distance + ", Damage: " + Mathf.Clamp(Mathf.RoundToInt(1 / distance * 25), 0, 32));
                print(h.transform.name);
                EnemyScript e = h.GetComponent<EnemyScript>();
                if (!e.stats.overrideDeath)
                    h.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(dir * 25 / distance * 3 * knockbackMultiplier, 25);
                //print("Distance: " + distance + ", Damage: " + damage);
                h.GetComponent<EnemyScript>().ReceiveDamage(damage);
                hitObjectNames.Add(h.transform.name);
            }

            if(h.tag == "Breakable")
            {
                Grabbable g = h.GetComponent<Grabbable>();
                g.TakeDamage(this.gameObject, damage);
            }
        }

        if(gibs.Length > 0)
            for(int i = 0; i < gibs.Length; i++)
            {
                Instantiate(gibs[i], transform.position, Quaternion.identity);
            }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
