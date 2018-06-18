using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

    public GameObject player;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 4;
    public static int radius = 1;
    public static Dictionary<string, Chunk> chunks;

    public Button playButton;
    public Slider loadingAmount;
    public Camera UICam;

    bool firstBuild = true;
    bool building = false;

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

        float totalChunks = firstBuild ? (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2 : 0;
        int processCount = 0;

        building = true;
        for (int z = -radius; z <= radius; z++)
        {
            for (int x =-radius; x <= radius; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 cPos = new Vector3((x + posX) * chunkSize, y * chunkSize, (z + posZ) * chunkSize);
                    string n = BuildChunkName(cPos);
                    Chunk c;

                    if (chunks.TryGetValue(n, out c))
                    {
                        c.status = Chunk.ChunkStatus.KEEP;
                        break;
                    }
                    else
                    {
                        c = new Chunk(cPos, textureAtlas);
                        c.chunk.transform.parent = this.transform;
                        chunks.Add(c.chunk.name, c);
                    }

                    if (firstBuild)
                    {
                        processCount++;
                        loadingAmount.value = processCount / totalChunks * 100;
                    }

                    yield return null;
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                c.Value.status = Chunk.ChunkStatus.KEEP;
            }

            // Delete chunks

            c.Value.status = Chunk.ChunkStatus.DONE;

            if (firstBuild)
            {
                processCount++;
                loadingAmount.value = processCount / totalChunks * 100;
            }

            yield return null;
        }

        if (firstBuild)
        {
            player.SetActive(true);
            loadingAmount.gameObject.SetActive(false);
            UICam.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            firstBuild = false;
        }
        building = false;
    }

    public void StartBuild()
    {
        StartCoroutine(BuildWorld());
    }

	// Use this for initialization
	void Start () {
        player.SetActive(false);
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
        if (!building && !firstBuild) StartCoroutine(BuildWorld());
	}
}
