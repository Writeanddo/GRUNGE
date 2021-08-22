﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibScript : MonoBehaviour
{
    public bool useStartingVelocity;
    public Vector2 velocity;
    public float magnitude;
    public float timeBeforeDestroy = 8;

    Collider2D col;
    Rigidbody2D rb;
    SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        Vector2 v;
        if (useStartingVelocity)
             v = velocity.normalized * magnitude;
        else
            v = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * magnitude;

        rb.velocity = v;
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.15f);
    }

    public void StopVelocity()
    {
        col.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(timeBeforeDestroy);
        while(spr.color.a > 0)
        {
            spr.color = new Color(1, 1, 1, spr.color.a - 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
    }
}