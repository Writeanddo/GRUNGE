using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChunkManager : MonoBehaviour
{
    public ChunkScript activeChunk;
    public ChunkScript previousChunk;
    public ChunkScript[] chunkPool;

    // Start is called before the first frame update
    void Start()
    {
        PlaceNeighboringChunk();
    }

    IEnumerator ChunkPlacer()
    {
        PlaceNeighboringChunk();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ChunkPlacer());
    }

    public void PlaceNeighboringChunk()
    {
        // Get random compass direction to place chunk at
        // Make sure we don't place a chunk where it's already connected to one
        int dir = -1;
        while(dir == -1)
        {
            int rand = Random.Range(0, 4);
            if (!activeChunk.occupiedEntrances[rand])
                dir = rand;
        }

        // Get next chunk object, avoid spawning duplicate chunks
        ChunkScript c = chunkPool[Random.Range(0, chunkPool.Length)];
        while(c == activeChunk)
            c = chunkPool[Random.Range(0, chunkPool.Length)];

        c = Instantiate(c).GetComponent<ChunkScript>();

        print(dir);
        c.InitializeChunkPosition(activeChunk.entrances[dir].position, dir);

        if(previousChunk != null)
            Destroy(previousChunk.gameObject);
        previousChunk = activeChunk;
        activeChunk = c;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
