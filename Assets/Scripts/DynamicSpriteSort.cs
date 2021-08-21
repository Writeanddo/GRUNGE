using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicSpriteSort : MonoBehaviour
{
    public bool overrideSort;
    public bool useParentPosition;
    public int sortMultiplier = 1;
    public float yOffset = 0;

    Transform player;
    SpriteRenderer spr;
    TilemapRenderer tile;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spr = GetComponent<SpriteRenderer>();
        tile = GetComponent<TilemapRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!overrideSort)
        {
            int sort = 0;
            float yPos;
            if (useParentPosition)
                yPos = transform.parent.position.y;
            else
                yPos = transform.position.y;
            sort = Mathf.RoundToInt((player.transform.position.y - yPos + yOffset) * 10) * sortMultiplier;

            if (spr != null)
                spr.sortingOrder = sort;
            else if (tile != null)
                tile.sortingOrder = sort;
        }
    }
}
