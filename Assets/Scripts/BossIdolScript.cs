using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdolScript : MonoBehaviour
{
    public Sprite[] gibSprites;
    public GameObject gibBase;
    public AudioClip hover;
    Animator anim;
    PlayerController ply;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        anim = GetComponent<Animator>();
        StartCoroutine(MovementCycle());
    }

    IEnumerator MovementCycle()
    {
        gm.PlaySFX(hover);
        float timer = 0;
        while(timer < 4)
        {
            transform.position = Vector2.Lerp(transform.position, ply.transform.position, 0.05f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.15f);
        anim.Play("BossTotemExplode");

    }

    public void BreakIntoGibs()
    {
        // Scan for player within circle
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 3);
        for(int i = 0; i < cols.Length; i++)
        {
            if (cols[i].tag == "Player")
                ply.ReceiveDamage(25);
        }
        
        for(int i = 0; i < gibSprites.Length; i++)
        {
            SpriteRenderer g = Instantiate(gibBase, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            g.sprite = gibSprites[i];
        }

        Destroy(this.gameObject);
    }
}
