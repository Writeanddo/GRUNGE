using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Rigidbody2D rb;
    Transform crosshair;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right*20;
    }
    void Launch(float angle)
    {
        

    }
}
