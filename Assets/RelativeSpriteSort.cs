using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeSpriteSort : MonoBehaviour
{
    public Renderer targetSpriteRenderer;
    public int layerOffset;

    SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spr.sortingOrder = targetSpriteRenderer.sortingOrder + layerOffset;
    }
}
