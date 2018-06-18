using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {

    public GameObject player;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 1;
    public static int radius = 4;
    public static ConcurrentDictionary<string, Chunk> chunks;

    bool firstBuild = true;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    void BuildChunkAt(int x, int y, int z)
    {
        Vector3 cPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        string n = BuildChunkName(cPos);
        Chunk c;

        if (!chunks.TryGetValue(n, out c))
        {
            c = new Chunk(cPos, textureAtlas);
            c.chunk.transform.parent = this.transform;
            chunks.TryAdd(c.chunk.name, c);
        }
    }

    IEnumerator BuildRecWorld(int x, int y, int z, int rad)
    {
        if (rad <= 0) yield break;

        BuildChunkAt(x, y, z - 1);
        StartCoroutine(BuildRecWorld(x, y, z - 1, --rad));

        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW) c.Value.DrawChunk();
            yield return null;
        }
    }

	// Use this for initialization
	void Start () {
        Vector3 ppos = player.transform.position;

        player.transform.position = new Vector3(ppos.x, Utils.GenHeight(ppos.x, ppos.z) + 1, ppos.z);
        player.SetActive(false);

        firstBuild = true;
        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        // Build starting chunk
        BuildChunkAt((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize));

        // Start Chunk drawing coroutine
        StartCoroutine(DrawChunks());

        // Create the rest of the world
        StartCoroutine(BuildRecWorld((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize), radius));
	}
	
	// Update is called once per frame
	void Update () {
        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstBuild = false;
        }

        StartCoroutine(DrawChunks());
	}
}
