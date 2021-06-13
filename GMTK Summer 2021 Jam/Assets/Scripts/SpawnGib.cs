using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGib : MonoBehaviour
{
    public GameObject gib;
    public Vector2 offset = new Vector2(0, -1);

    public void Spawn()
    {
        Instantiate(gib, transform.position + (Vector3)offset, transform.rotation);
    }
}
