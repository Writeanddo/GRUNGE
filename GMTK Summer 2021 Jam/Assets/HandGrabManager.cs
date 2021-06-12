using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabManager : MonoBehaviour
{
    PlayerController player;
    Animator anim;
    DynamicSpriteSort heldObjectSort;
    SpriteRenderer heldObjectSpr;
    Grabbable heldObjectGrabbable;
    SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.heldObject != null)
        {
            heldObjectSpr.sortingOrder = spr.sortingOrder - 1;
        }
    }

    public void ThrowItem(Vector3 velocity)
    {
        player.heldObject.transform.parent = null;

        heldObjectGrabbable.SetHeldState(false);
        heldObjectGrabbable.rb.velocity = velocity;
        heldObjectSort.overrideSort = false;

        player.heldObject = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player.handLaunched)
        {
            if (collision.tag == "Grabbable")
            {
                collision.transform.parent = transform;
                collision.transform.localPosition = new Vector2(0, -0.2f);
                player.heldObject = collision.transform;

                heldObjectGrabbable = collision.GetComponent<Grabbable>();
                heldObjectSort = collision.GetComponent<DynamicSpriteSort>();
                heldObjectSpr = collision.GetComponent<SpriteRenderer>();
                heldObjectSort.overrideSort = true;
                heldObjectGrabbable.SetHeldState(true);

                player.handLaunched = false;

            }
            // Stop extending hand if we hit a wall
            else if (collision.tag == "Wall")
                player.handLaunched = false;
        }
    }

    public void CheckAndPlayClip(string clipName)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }
}
