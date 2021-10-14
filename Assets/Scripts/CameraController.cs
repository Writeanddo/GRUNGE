using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform secondTarget;
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
            Vector3 targetPosition = target.transform.position;
            if(secondTarget != null)
            {
                float newX = (secondTarget.position.x + target.transform.position.x) / 2;
                float newY = target.position.y - (target.transform.position.y - secondTarget.transform.position.y) / 3;
                targetPosition = new Vector2(newX, newY);
            }

            // Make sure we don't go outside screen bounds
            transform.position = new Vector3(Mathf.Clamp(targetPosition.x, minX, maxX), Mathf.Clamp(targetPosition.y, minY, maxY), transform.position.z);
        }

        // After camera pos is determined
        crosshair.UpdateCrosshair();
    }
}
