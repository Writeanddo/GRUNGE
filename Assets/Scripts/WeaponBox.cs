using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    public int gunIndex;
    public bool dontDisappear;

    SpriteRenderer spr;
    GameManager gm;
    Animator anim;
    AudioSource audio;
    PlayerController ply;
    Grabbable g;

    // Start is called before the first frame update
    void Start()
    {
        g = GetComponent<Grabbable>();
        ply = FindObjectOfType<PlayerController>();
        spr = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        audio = GetComponentInChildren<AudioSource>();

        switch(gunIndex)
        {
            case (1):
                anim.Play("ShotgunBox");
                break;
            case (2):
                anim.Play("TommygunBox");
                break;
            case (10):
                anim.Play("ScytheBox");
                break;
            case (11):
                anim.Play("KnuckleBox");
                break;
        }

        if(!dontDisappear)
            StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(15);
        for(int i = 0; i < 25; i++)
        {
            spr.color = new Color(1, 1, 1, 0.35f);
            yield return new WaitForSeconds(0.1f);
            spr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (g.isHeld && Vector3.Distance(transform.position, ply.transform.position) < 1.5f)
        {
            EquipGun();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            EquipGun();
        }
    }

    public void PlayWeaponBoxSound()
    {
        audio.PlayOneShot(gm.generalSfx[11]);
    }

    void EquipGun()
    {
        if (ply.heldObject == this.gameObject)
            ply.heldObject = null;
        else if (gunIndex >= 10 && ply.heldObject != null)
            FindObjectOfType<HandGrabManager>().DropItem();

        gm.PickupGun(gunIndex);
        if (ply.heldObject == this.gameObject)
            ply.heldObject = null;
        Destroy(this.gameObject);
    }
}
