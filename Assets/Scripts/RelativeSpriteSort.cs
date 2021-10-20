using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeSpriteSort : MonoBehaviour
{
    public Renderer targetSpriteRenderer;
    public int layerOffset;

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rend.sortingOrder = targetSpriteRenderer.sortingOrder + layerOffset;
    }
}
