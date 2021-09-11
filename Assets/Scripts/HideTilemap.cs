using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideTilemap : MonoBehaviour
{
    float transparency = 1;
    public float yOffset;
    Tilemap t;
    TilemapRenderer rend;
    PlayerController player;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rend = GetComponent<TilemapRenderer>();
        t = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t.color = Color.Lerp(t.color, new Color(1, 1, 1, transparency), 0.25f);
        //rend.sortingOrder = Mathf.RoundToInt((player.transform.position.y - transform.position.y + yOffset) * 10);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            transparency = 0.25f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            transparency = 1;
    }
}
