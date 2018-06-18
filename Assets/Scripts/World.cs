using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public GameObject player;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 4;
    public static int radius = 1;
    public static Dictionary<string, Chunk> chunks;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    IEnumerator BuildChunkColumn()
    {
        for (int i = 0; i < columnHeight; i++)
        {
            Vector3 cPos = new Vector3(this.transform.position.x, i * chunkSize, this.transform.position.z);
            Chunk c = new Chunk(cPos, textureAtlas);

            c.chunk.transform.parent = this.transform;
            chunks.Add(c.chunk.name, c);
        }

        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }

    IEnumerator BuildWorld()
    {
        int posX = (int)Mathf.Floor(player.transform.position.x / chunkSize);
        int posZ = (int)Mathf.Floor(player.transform.position.z / chunkSize);

        for (int z = -radius; z <= radius; z++)
        {
            for (int x =-radius; x <= radius; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 cPos = new Vector3((x + posX) * chunkSize, y * chunkSize, (z + posZ) * chunkSize);
                    Chunk c = new Chunk(cPos, textureAtlas);

                    c.chunk.transform.parent = this.transform;
                    chunks.Add(c.chunk.name, c);
                    yield return null;
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
        player.SetActive(true);
    }

	// Use this for initialization
	void Start () {
        player.SetActive(false);
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
