using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3((target.transform.position.x), target.transform.position.y, transform.position.z);
            //float x = Mathf.Round(transform.position.x * 16) / 16;
            //float y = Mathf.Round(transform.position.y * 16) / 16;
            //transform.position = new Vector3(x, y, transform.position.z);
        }

    }
}
