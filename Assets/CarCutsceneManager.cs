using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCutsceneManager : MonoBehaviour
{
    public AudioClip[] sfx;

    Animator anim;
    PlayerController ply;
    GameManager gm;

    bool hitDude;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        ply = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();

        StartCoroutine(CarCutscene());
    }

    IEnumerator CarCutscene()
    {
        yield return new WaitForSeconds(1.5f);
        anim.Play("CarApproach");
        yield return new WaitForSeconds(0.25f);
        gm.PlaySFX(sfx[0]);
        yield return new WaitForSeconds(1.75f);
        gm.PlaySFX(sfx[2]);
        ply.SetVisible();
        ply.transform.parent = null;
        yield return new WaitForSeconds(1f);
        yield return gm.WaitForTextCompletion();
        gm.PlaySFX(sfx[1]);
        anim.Play("CarLeave");
        yield return new WaitForSeconds(1f);
        ply.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !hitDude)
        {
            hitDude = true;
            collision.GetComponent<EnemyScript>().ExplodeIntoGoo();
            //gm.PlaySFX(sfx[2]);
            gm.PlaySFX(gm.generalSfx[3]);
        }
    }
}
