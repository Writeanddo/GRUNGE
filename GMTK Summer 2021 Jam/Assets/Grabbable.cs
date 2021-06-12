using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isHeld;
    [HideInInspector]
    public Rigidbody2D rb;
    Collider2D col;
    int defaultLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        defaultLayer = gameObject.layer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHeld)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.15f);
        }
    }

    public void SetHeldState(bool state)
    {
        isHeld = state;
        rb.isKinematic = state;

        if (!state)
        {
            StartCoroutine(ColliderEnableDelay());
        }
        else
            gameObject.layer = 8;
    }

    IEnumerator ColliderEnableDelay()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = defaultLayer;
    }
}
