using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public Camera cam;
    public Transform helper;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, 0);
        helper.position = new Vector3((player.position.x + transform.position.x) / 2, (player.position.y + transform.position.y) / 2);
    }
}
