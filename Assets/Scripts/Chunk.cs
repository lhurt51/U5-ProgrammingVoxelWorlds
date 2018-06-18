﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    public Material cubeMat;
    public Block[,,] chunkData;
    public GameObject chunk;

    void BuildChunk()
    {
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

                    if (worldY == 0) chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, chunk.gameObject, this);
                    else if (Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f) chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    else if (worldY <= Utils.GenStoneHeight(worldX, worldZ))
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.325f && worldY < 40) chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.03f, 3) < 0.35f && worldY < 20) chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos, chunk.gameObject, this);
                        else chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);
                    }
                    else if (worldY == Utils.GenHeight(worldX, worldZ)) chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);
                    else if (worldY < Utils.GenHeight(worldX, worldZ)) chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);
                    else chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                }
            }
        }
    }

    void CombineMeshes()
    {
        // Combine all children meshes
        int i = 0;
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i++].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        // Create a renderer for the parent
        MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        mf.mesh = new Mesh();
        // Add combined meshes on children as the parents mesh
        mf.mesh.CombineMeshes(combine);

        renderer.material = cubeMat;

        // Delete all uncombined children
        foreach (Transform quad in chunk.transform) GameObject.Destroy(quad.gameObject);
    }

    public void DrawChunk()
    {
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
        CombineMeshes();
    }

    public Chunk(Vector3 pos, Material c)
    {
        chunk = new GameObject(World.BuildChunkName(pos));
        chunk.transform.position = pos;
        cubeMat = c;
        BuildChunk();
    }

}