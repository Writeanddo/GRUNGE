using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    public Transform[] entrances; // 0 = N, 1 = S, 2 = E, 3 = W
    public bool[] occupiedEntrances;

    LevelChunkManager lcm;

    // Start is called before the first frame update
    void Start()
    {
        lcm = FindObjectOfType<LevelChunkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeChunkPosition(Vector3 entrancePosition, int entranceDirection)
    {
        // Position should be set to opposite of given entrance direction (e.g. if provided north, we set to south)
        Vector3 offset = Vector3.zero;
        switch(entranceDirection)
        {
            case 0:
                occupiedEntrances[1] = true;
                offset = entrances[1].position - transform.position;
                break;
            case 1:
                occupiedEntrances[0] = true;
                offset = entrances[0].position - transform.position;
                break;
            case 2:
                occupiedEntrances[3] = true;
                offset = entrances[3].position - transform.position;
                break;
            case 3:
                occupiedEntrances[2] = true;
                offset = entrances[2].position - transform.position;
                break;
        }

        transform.position = entrancePosition - offset;
    }
}
