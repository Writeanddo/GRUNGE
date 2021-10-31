using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCutsceneManager : MonoBehaviour
{
    public AudioClip[] sfx;
    public Sprite[] carSprites;
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
        while (gm == null)
        {
            anim = GetComponent<Animator>();
            ply = FindObjectOfType<PlayerController>();
            gm = FindObjectOfType<GameManager>();
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.25f);
        gm.SetCrosshairVisibility(false);
        yield return new WaitForSeconds(1.5f);
        anim.Play("CarApproach");
        yield return new WaitForSeconds(0.25f);
        gm.PlaySFX(sfx[0]);
        yield return new WaitForSeconds(1.75f);
        anim.Play("CarDoorOpen");
        gm.PlaySFX(sfx[2]);
        ply.SetVisible();
        ply.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 15);
        StartCoroutine(StopPlayer());
        ply.transform.parent = null;
        if (!gm.playingEndlessMode)
        {
            yield return new WaitForSeconds(1f);
            yield return gm.WaitForTextCompletion("ExitCar");
        }
        gm.PlaySFX(sfx[1]);
        anim.Play("CarLeave");
        yield return new WaitForSeconds(1f);
        ply.canMove = true;
        gm.canPause = true;
        gm.SetCrosshairVisibility(true);
    }

    IEnumerator StopPlayer()
    {
        Rigidbody2D rb = ply.GetComponent<Rigidbody2D>();

        while(rb.velocity.y > 0.05f)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.15f);
            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector2.zero;
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
