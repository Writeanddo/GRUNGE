using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpriteSort : MonoBehaviour
{
    public bool overrideSort;
    public bool useParentPosition;
    public int sortMultiplier = 1;
    public float yOffset = 0;

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
        if (!overrideSort)
        {
            float yPos;
            if (useParentPosition)
                yPos = transform.parent.position.y;
            else
                yPos = transform.position.y;
            spr.sortingOrder = Mathf.RoundToInt((player.transform.position.y - yPos + yOffset) * 10) * sortMultiplier;
        }
    }
}
