using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    List<Transform> cachedTransforms;
    List<Transform> cachedEdgeTransforms;

    Collider2D c;

    // Start is called before the first frame update
    void Start()
    {
        cachedTransforms = new List<Transform>();
        cachedEdgeTransforms = new List<Transform>();
        c = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform t in cachedTransforms)
        {
            if (t.position.y < transform.position.y)
                t.position = c.ClosestPoint(t.position);
            else
                t.position += Vector3.down * 0.3f;
        }

        foreach(Transform t in cachedEdgeTransforms)
        {
            t.position = c.ClosestPoint(t.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
            cachedTransforms.Add(collision.transform.parent);
        else if (collision.transform.tag == "Enemy" || collision.transform.tag == "Grabbable")
            cachedTransforms.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && cachedTransforms.Contains(collision.transform.parent))
            cachedTransforms.Remove(collision.transform.parent);
        else if ((collision.transform.tag == "Enemy" || collision.transform.tag == "Grabbable") && cachedTransforms.Contains(collision.transform))
            cachedTransforms.Remove(collision.transform);
    }
}