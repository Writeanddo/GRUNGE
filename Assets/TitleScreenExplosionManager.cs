using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenExplosionManager : MonoBehaviour
{
    public AudioClip explosionSound;
    AudioSource sfx;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
    }

    public void Explode()
    {
        transform.position = Vector2.zero;
        sfx.PlayOneShot(explosionSound);
        anim.Play("ExplosionEnemy", -1, 0);
    }
}
