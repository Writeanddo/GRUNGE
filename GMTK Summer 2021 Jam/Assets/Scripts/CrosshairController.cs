using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public Camera cam;
    public Transform helper;
    public Transform player;

    SpriteRenderer crosshair;
    SpriteRenderer miniCrosshair;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = GetComponent<SpriteRenderer>();
        miniCrosshair = GameObject.Find("SubCrosshair").GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.paused)
            return;

        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, 0);

        Vector3 dir = (transform.position - player.position);

        if (dir.magnitude < 6)
            helper.position = mouse;
        else
            helper.position = player.position + dir.normalized * 6;
    }

    public void SetVisible(bool state)
    {
        if(!state)
        {
            crosshair.color = Color.clear;
            miniCrosshair.color = Color.clear;
        }
        else
        {
            crosshair.color = Color.white;
            miniCrosshair.color = Color.white;
        }
    }
}
