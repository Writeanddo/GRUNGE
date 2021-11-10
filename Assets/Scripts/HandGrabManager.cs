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
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.heldObject != null)
        {
            heldObjectSpr.sortingOrder = spr.sortingOrder - 1;
            player.heldObject.transform.localPosition = heldObjectGrabbable.objectOffsetWhenHeld;
            transform.localPosition = heldObjectGrabbable.handOffsetWhenHeld;
        }
        else
            transform.localPosition = Vector2.zero;
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
        if (player.heldObject == null)
            return;
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
                GrabObject(collision.gameObject);

            }
            // Stop extending hand if we hit a wall
            else if (collision.tag == "Wall")
                player.handLaunched = false;
        }
    }

    public void InitialGrabbableScan()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.67f);
        foreach (Collider2D c in cols)
        {
            if (c.tag == "Grabbable" || c.tag == "Enemy")
            {
                GrabObject(c.gameObject);
                break;
            }
            // Stop extending hand if we hit a wall
            /*else if (c.tag == "Wall")
            {
                player.handLaunched = false;
                break;
            }*/
        }
    }

    public void GrabObject(GameObject g)
    {
        print("Grabbed " + g.name);
        heldObjectGrabbable = g.GetComponentInChildren<Grabbable>();
        heldObjectGrabbable.Initialize();
        if (!heldObjectGrabbable.canBeGrabbed)
        {
            player.handLaunched = false;
            return;
        }

        gm.PlaySFX(gm.playerSfx[3], 1);
        g.transform.parent = transform;
        player.heldObject = g.transform;

        heldObjectSort = g.GetComponentInChildren<DynamicSpriteSort>();
        heldObjectSpr = g.GetComponentInChildren<SpriteRenderer>();
        heldObjectSort.overrideSort = true;
        heldObjectGrabbable.SetHeldState(true);

        player.handLaunched = false;
    }

    public void CheckAndPlayClip(string clipName)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }
}
