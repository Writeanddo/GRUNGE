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
            player.heldObject.transform.localPosition = heldObjectGrabbable.objectOffsetWhenHeld;
            transform.localPosition = heldObjectGrabbable.handOffsetWhenHeld;
        }
    }

    public void ThrowItem(Vector3 velocity)
    {
        player.heldObject.transform.parent = null;
        transform.localPosition = Vector2.zero;

        heldObjectGrabbable.SetHeldState(false);
        heldObjectGrabbable.rb.velocity = velocity;
        heldObjectSort.overrideSort = false;

        heldObjectGrabbable.isBeingThrown = true;
        if (player.heldObject.tag == "Enemy")
            player.heldObject.SendMessage("GetThrown");

        player.heldObject = null;
    }

    public void DropItem()
    {
        player.heldObject.transform.parent = null;
        transform.localPosition = Vector2.zero;

        heldObjectGrabbable.SetHeldState(false);
        heldObjectSort.overrideSort = false;

        heldObjectGrabbable.isBeingThrown = true;
        if (player.heldObject.tag == "Enemy")
            player.heldObject.SendMessage("GetThrown");

        player.heldObject = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player.handLaunched)
        {
            if (collision.tag == "Grabbable" || collision.tag == "Enemy")
            {
                heldObjectGrabbable = collision.GetComponentInChildren<Grabbable>();
                if (!heldObjectGrabbable.canBeGrabbed)
                    return;

                collision.transform.parent = transform;
                player.heldObject = collision.transform;
                
                heldObjectSort = collision.GetComponentInChildren<DynamicSpriteSort>();
                heldObjectSpr = collision.GetComponentInChildren<SpriteRenderer>();
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
