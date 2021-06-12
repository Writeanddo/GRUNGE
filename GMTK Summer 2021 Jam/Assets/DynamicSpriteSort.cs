using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpriteSort : MonoBehaviour
{
    public bool overrideSort;
    Transform player;
    SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!overrideSort)
            spr.sortingOrder = Mathf.RoundToInt((player.transform.position.y - transform.position.y) * 10);
    }
}
