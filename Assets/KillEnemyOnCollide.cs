using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemyOnCollide : MonoBehaviour
{
    public bool active = false;
    GameManager gm;
    bool hitDude;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !hitDude && active)
        {
            hitDude = true;
            collision.GetComponent<EnemyScript>().ExplodeIntoGoo();
            //gm.PlaySFX(sfx[2]);
            gm.PlaySFX(gm.generalSfx[3]);
        }
    }
}
