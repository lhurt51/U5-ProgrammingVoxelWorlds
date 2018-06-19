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
    public static List<string> toRemove = new List<string>();

    bool firstBuild = true;
    Vector3 lastBuildPos;

    CoroutineQueue queue;
    public static uint maxCoroutines = 1000;

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
        rad--;
        if (rad <= 0) yield break;

        // Build chunk frnt
        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecWorld(x, y, z + 1, rad));
        yield return null;

        // Build chunk back
        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecWorld(x, y, z - 1, rad));
        yield return null;

        // Build chunk left
        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecWorld(x - 1, y, z, rad));
        yield return null;

        // Build chunk right
        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecWorld(x + 1, y, z, rad));
        yield return null;

        // Build chunk up
        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecWorld(x, y + 1, z, rad));
        yield return null;

        // Build chunk down
        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecWorld(x, y - 1, z, rad));
        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW) c.Value.DrawChunk();
            if (c.Value.chunk && Vector3.Distance(player.transform.position, c.Value.chunk.transform.position) > radius * chunkSize) toRemove.Add(c.Key);
            yield return null;
        }
    }

    IEnumerator RemoveOldChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            Chunk c;
            string n = toRemove[i];

            if (chunks.TryGetValue(n, out c))
            {
                Destroy(c.chunk);
                c.Save();
                chunks.TryRemove(n, out c);
                yield return null;
            }
        }
    }

    public void BuildNearPlayer()
    {
        StopCoroutine("BuildRecWorld");
        queue.Run(BuildRecWorld((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize), radius));
    }

	// Use this for initialization
	void Start () {
        Vector3 ppos = player.transform.position;

        player.transform.position = new Vector3(ppos.x, Utils.GenHeight(ppos.x, ppos.z) + 1, ppos.z);
        lastBuildPos = player.transform.position;
        player.SetActive(false);

        firstBuild = true;
        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        // Build starting chunk
        BuildChunkAt((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize));

        // Start Chunk drawing coroutine
        queue.Run(DrawChunks());

        // Create the rest of the world
        queue.Run(BuildRecWorld((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize), radius));
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 movement = lastBuildPos - player.transform.position;

        if (movement.magnitude > chunkSize)
        {
            lastBuildPos = player.transform.position;
            BuildNearPlayer();
        }

        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstBuild = false;
        }

        queue.Run(DrawChunks());
        queue.Run(RemoveOldChunks());
	}
}
