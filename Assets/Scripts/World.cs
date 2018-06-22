using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {

    public GameObject player;
    public Material textureAtlas;
    public Material fluidTex;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 1;
    public static int radius = 2;
    public static ConcurrentDictionary<string, Chunk> chunks;
    public static List<string> toRemove = new List<string>();

    Vector3 lastBuildPos;

    static CoroutineQueue queue;
    public static uint maxCoroutines = 1000;

    public static CoroutineQueue Queue
    {
        get { return queue; }
    }

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    public static Block GetWorldBlock(Vector3 pos)
    {
        int cx, cy, cz;

        if (pos.x < 0) cx = (int)((Mathf.Round(pos.x - chunkSize) + 1) / (float)chunkSize) * chunkSize;
        else cx = (int)(Mathf.Round(pos.x) / (float)chunkSize) * chunkSize;

        if (pos.y < 0) cy = (int)((Mathf.Round(pos.y - chunkSize) + 1) / (float)chunkSize) * chunkSize;
        else cy = (int)(Mathf.Round(pos.y) / (float)chunkSize) * chunkSize;

        if (pos.z < 0) cz = (int)((Mathf.Round(pos.z - chunkSize) + 1) / (float)chunkSize) * chunkSize;
        else cz = (int)(Mathf.Round(pos.z) / (float)chunkSize) * chunkSize;

        Chunk c;
        string cn = BuildChunkName(new Vector3(cx, cy, cz));
        int blx = (int)Mathf.Abs((float)Mathf.RoundToInt(pos.x) - cx);
        int bly = (int)Mathf.Abs((float)Mathf.RoundToInt(pos.y) - cy);
        int blz = (int)Mathf.Abs((float)Mathf.RoundToInt(pos.z) - cz);

        if (chunks.TryGetValue(cn, out c)) return c.chunkData[blx, bly, blz];
        else return null;
    }

    void BuildChunkAt(int x, int y, int z)
    {
        Vector3 cPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        string n = BuildChunkName(cPos);
        Chunk c;

        if (!chunks.TryGetValue(n, out c))
        {
            c = new Chunk(cPos, textureAtlas, fluidTex);
            c.chunk.transform.parent = this.transform;
            if (c.fluid != null) c.fluid.transform.parent = this.transform;
            chunks.TryAdd(c.chunk.name, c);
        }
    }

    IEnumerator BuildRecWorld(int x, int y, int z, int startRad, int rad)
    {
        int nextrad = rad - 1;
        if (rad <= 0 || y < 0 || y > columnHeight) yield break;

        // Main chunk
        if (rad == radius)
        {
            BuildChunkAt(x, y, z);
            yield return null;
        }

        // Build chunk frnt
        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecWorld(x, y, z + 1, rad, nextrad));

        // Build chunk back
        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecWorld(x, y, z - 1, rad, nextrad));

        // Build chunk left
        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecWorld(x - 1, y, z, rad, nextrad));

        // Build chunk right
        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecWorld(x + 1, y, z, rad, nextrad));

        // Build chunk up
        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecWorld(x, y + 1, z, rad, nextrad));

        // Build chunk down
        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecWorld(x, y - 1, z, rad, nextrad));
    }

    IEnumerator DrawChunks()
    {
        yield return null;

        toRemove.Clear();
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW) c.Value.DrawChunk();
            if (c.Value.chunk && Vector3.Distance(player.transform.position, c.Value.chunk.transform.position) > radius * chunkSize) toRemove.Add(c.Key);
        }
        yield break;
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
                Destroy(c.fluid);
                // c.Save();
                chunks.TryRemove(n, out c);
            }
        }
        yield break;
    }

    public void BuildNearPlayer()
    {
        StopCoroutine("BuildRecWorld");
        queue.Run(BuildRecWorld((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize), radius, radius));
    }

	// Use this for initialization
	void Start () {
        Vector3 ppos = player.transform.position;

        player.transform.position = new Vector3(ppos.x, Utils.GenHeight(ppos.x, ppos.z) + 5, ppos.z);
        lastBuildPos = player.transform.position;
        player.SetActive(false);

        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        // Build starting chunk
        // BuildChunkAt((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize));

        // Start Chunk drawing coroutine
        // queue.Run(DrawChunks());

        // Create the rest of the world
        queue.Run(BuildRecWorld((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize), radius, radius));
    }

    // Update is called once per frame
    void Update () {
        Vector3 movement = lastBuildPos - player.transform.position;

        if (movement.magnitude >= chunkSize * 0.75)
        {
            lastBuildPos = player.transform.position;
            BuildNearPlayer();
        }

        if (!player.activeSelf) player.SetActive(true);

        queue.Run(DrawChunks());
        queue.Run(RemoveOldChunks());
    }
}
