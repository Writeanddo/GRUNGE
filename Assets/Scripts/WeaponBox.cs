using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    public int gunIndex;

    GameManager gm;
    Animator anim;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
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
            case (11):
                anim.Play("KnuckleBox");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gm.PickupGun(gunIndex);
            Destroy(this.gameObject);
        }
    }

    public void PlayWeaponBoxSound()
    {
        audio.PlayOneShot(gm.generalSfx[11]);
    }
}
