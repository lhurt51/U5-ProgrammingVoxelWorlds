using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
class BlockData
{
    public Block.BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] b)
    {
        matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    matrix[x, y, z] = b[x, y, z].bType;
                }
            }
        }
    }
}

public class Chunk {

    public enum ChunkStatus
    {
        DRAW,
        DONE,
        KEEP
    };
    public ChunkStatus status;
    public Material cubeMat;
    public Material fluidMat;
    public Block[,,] chunkData;
    public GameObject chunk;
    public GameObject fluid;
    public ChunkMB mb;
    public bool changed = false;

    BlockData bd;

    string BuildChunkFileName(Vector3 v)
    {
        return Application.persistentDataPath + "/savedata/Chunk_" + (int)v.x + "_" + (int)v.y + "_" + (int)v.z + "_" + World.chunkSize + "_" + World.radius + ".dat";
    }

    bool Load()
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);

        if (File.Exists(chunkFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.Open);

            bd = new BlockData();
            bd = (BlockData)bf.Deserialize(file);
            file.Close();

            // Debug.Log("Loading chunk from file: " + chunkFile);

            return true;
        }
        return false;
    }

    public void Save()
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);

        if (!File.Exists(chunkFile)) Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);

        bd = new BlockData(chunkData);
        bf.Serialize(file, bd);
        file.Close();

        // Debug.Log("Saving chunk from file: " + chunkFile);
    }

    void BuildChunk()
    {
        bool dataFromFile = Load();

        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    if (dataFromFile)
                    {
                        chunkData[x, y, z] = new Block(bd.matrix[x, y, z], pos, chunk.gameObject, this);
                        continue;
                    }

                    int surfaceHeight = Utils.GenHeight(worldX, worldZ);
                    if (worldY < 5) chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, chunk.gameObject, this);
                    else if (worldY <= Utils.GenStoneHeight(worldX, worldZ))
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY < 40) chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20) chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos, chunk.gameObject, this);
                        else chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);
                    }
                    else if (worldY == surfaceHeight) chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);
                    else if (worldY < surfaceHeight) chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);
                    else chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);

                    if (chunkData[x, y, z].bType != Block.BlockType.WATER && Utils.fBM3D(worldX, worldY, worldZ, 0.08f, 3) < 0.42f) chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    if (worldY < 70 && chunkData[x, y, z].bType == Block.BlockType.AIR) chunkData[x, y, z] = new Block(Block.BlockType.WATER, pos, fluid.gameObject, this);
                    if (worldY == 0) chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, chunk.gameObject, this);

                    status = ChunkStatus.DRAW;
                }
            }
        }
    }

    void CombineMeshes(GameObject o, Material m)
    {
        // Combine all children meshes
        int i = 0;
        MeshFilter[] meshFilters = o.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i++].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)o.gameObject.AddComponent(typeof(MeshFilter));
        // Create a renderer for the parent
        MeshRenderer renderer = o.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        mf.mesh = new Mesh();
        // Add combined meshes on children as the parents mesh
        mf.mesh.CombineMeshes(combine);
        renderer.material = m;

        // Delete all uncombined children
        foreach (Transform quad in o.transform) GameObject.Destroy(quad.gameObject);
    }

    public void DrawChunk()
    {
        MeshCollider col = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
                }
            }
        }
        CombineMeshes(chunk.gameObject, cubeMat);
        col.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
        CombineMeshes(fluid.gameObject, fluidMat);
        status = ChunkStatus.DONE;
    }

    public void Redraw()
    {
        GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
        GameObject.DestroyImmediate(chunk.GetComponent<Collider>());
        GameObject.DestroyImmediate(fluid.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(fluid.GetComponent<MeshRenderer>());
        DrawChunk();
    }

    public Chunk() { }

    public Chunk(Vector3 pos, Material c, Material t)
    {
        chunk = new GameObject(World.BuildChunkName(pos));
        chunk.transform.position = pos;

        fluid = new GameObject(World.BuildChunkName(pos) + "_F");
        fluid.transform.position = pos;

        mb = chunk.AddComponent<ChunkMB>();
        mb.SetOwner(this);

        cubeMat = c;
        fluidMat = t;

        BuildChunk();
    }

}
