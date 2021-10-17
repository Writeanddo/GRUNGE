using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveGibsNearBoss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10 || collision.gameObject.layer == 11)
        {
            Destroy(collision.gameObject);
        }
    }
}
