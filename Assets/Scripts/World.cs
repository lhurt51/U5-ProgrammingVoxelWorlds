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

        float totalChunks = (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2;
        int processCount = 0;

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

                    processCount++;
                    loadingAmount.value = processCount / totalChunks * 100;

                    yield return null;
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();

            processCount++;
            loadingAmount.value = processCount / totalChunks * 100;

            yield return null;
        }

        player.SetActive(true);
        loadingAmount.gameObject.SetActive(false);
        UICam.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
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
		
	}
}
