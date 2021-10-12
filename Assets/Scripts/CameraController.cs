using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public bool overridePosition;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    CrosshairController crosshair;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = FindObjectOfType<CrosshairController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null && !overridePosition)
        {
            // Make sure we don't go outside screen bounds
            transform.position = new Vector3(Mathf.Clamp(target.transform.position.x, minX, maxX), Mathf.Clamp(target.transform.position.y, minY, maxY), transform.position.z);
        }

        // After camera pos is determined
        crosshair.UpdateCrosshair();
    }
}
