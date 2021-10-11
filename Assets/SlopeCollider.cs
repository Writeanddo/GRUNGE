using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeCollider : MonoBehaviour
{
    public List<Rigidbody2D> cachedRigidBodies;

    // Start is called before the first frame update
    void Start()
    {
        cachedRigidBodies = new List<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(Rigidbody2D rb in cachedRigidBodies)
        {
            rb.AddForce(Vector2.down * rb.mass*10);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.transform.name);
        if(collision.transform.tag == "Player")
            cachedRigidBodies.Add(collision.transform.GetComponentInParent<Rigidbody2D>());
        else if (collision.transform.tag == "Enemy")
            cachedRigidBodies.Add(collision.transform.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.transform.tag == "Player" || collision.transform.tag == "Enemy") && cachedRigidBodies.Contains(collision.GetComponent<Rigidbody2D>()))
            cachedRigidBodies.Remove(collision.GetComponent<Rigidbody2D>());
    }
}
