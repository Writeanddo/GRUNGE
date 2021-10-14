using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    List<Transform> cachedTransforms;
    Collider2D c;

    // Start is called before the first frame update
    void Start()
    {
        cachedTransforms = new List<Transform>();
        c = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cachedTransforms.RemoveAll(item => item == null);
        foreach (Transform t in cachedTransforms)
        {
            if (t.position.y < transform.position.y)
                t.position = c.ClosestPoint(t.position);
            else
                t.position += Vector3.down * 0.3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            cachedTransforms.Add(collision.transform.parent);
        else if (collision.transform.tag == "Enemy")
            cachedTransforms.Add(collision.transform);
        else if (collision.transform.tag == "Grabbable")
            collision.GetComponent<Rigidbody2D>().velocity *= -1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && cachedTransforms.Contains(collision.transform.parent))
            cachedTransforms.Remove(collision.transform.parent);
        else if ((collision.transform.tag == "Enemy" || collision.transform.tag == "Grabbable") && cachedTransforms.Contains(collision.transform))
            cachedTransforms.Remove(collision.transform);
    }
}