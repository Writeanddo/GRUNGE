using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGib : MonoBehaviour
{
    public GameObject[] gibs;
    public Vector2 offset = new Vector2(0, -1);

    public void Spawn()
    {
        foreach(GameObject g in gibs)
            Instantiate(g, transform.position + (Vector3)offset, transform.rotation);
    }
}
