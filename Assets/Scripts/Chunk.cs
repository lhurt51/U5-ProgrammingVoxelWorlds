using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public Material cubeMat;
    public Block[,,] chunkData;

    public IEnumerator BuildWorld(int sizeX, int sizeY, int sizeZ)
    {
        chunkData = new Block[sizeX, sizeY, sizeZ];

        // Create blocks
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (Random.Range(0.0f, 100.0f) < 50.0f) chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, this.gameObject, cubeMat);
                    else chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this.gameObject, cubeMat);
                }
            }
        }

        // Draw blocks
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    chunkData[x, y, z].Draw();
                }
            }
            yield return null;
        }
        CombineMeshes();
    }

    void CombineMeshes()
    {
        // Combine all children meshes
        int i = 0;
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i++].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        // Create a renderer for the parent
        MeshRenderer renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        mf.mesh = new Mesh();
        // Add combined meshes on children as the parents mesh
        mf.mesh.CombineMeshes(combine);

        renderer.material = cubeMat;

        // Delete all uncombined children
        foreach (Transform quad in this.transform) Destroy(quad.gameObject);
    }

    // Use this for initialization
    void Start () {
        int size = 16;
        StartCoroutine(BuildWorld(size, size, size));
    }
}
