using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum BlockType
    {
        GRASS,
        DIRT,
        STONE,
        BEDROCK,
        REDSTONE,
        DIAMOND,
        NOCRACK,
        CRACK1,
        CRACK2,
        CRACK3,
        CRACK4,
        CRACK5,
        CRACK6,
        CRACK7,
        CRACK8,
        CRACK9,
        CRACK10,
        AIR
    };

    public BlockType bType;
    public BlockType health;
    public bool isSolid;

    public Chunk Owner
    {
        get { return owner; }
    }

    public Vector3 Pos
    {
        get { return pos; }
    }

    enum CubeSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    Chunk owner;
    GameObject parent;
    Vector3 pos;

    int curHealth;
    int[] blockHealthMax = { 6, 6, 9, -1, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    Vector2[,] blockUVs =
    {
        // Grass top
        { new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f), new Vector2(0.125f, 0.4275f), new Vector2(0.1875f, 0.4375f) },
        // Grass side
        { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f) },
        // Dirt
        { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f) },
        // Stone
        { new Vector2(0.0f, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0.0f, 0.9375f), new Vector2(0.0625f, 0.9375f) },
        // Bedrock
        { new Vector2(0.3125f, 0.8125f), new Vector2(0.375f, 0.8125f), new Vector2(0.3125f, 0.875f), new Vector2(0.375f, 0.875f) },
		// Redstone
        { new Vector2(0.1875f, 0.75f), new Vector2(0.25f, 0.75f), new Vector2 (0.1875f, 0.8125f), new Vector2(0.25f, 0.8125f) },
        // Diamond
        { new Vector2(0.125f, 0.75f), new Vector2(0.1875f, 0.75f), new Vector2(0.125f, 0.8125f), new Vector2(0.1875f, 0.8125f) },
        // No Crack
        { new Vector2(0.6875f, 0.0f),  new Vector2(0.75f, 0.0f), new Vector2(0.6875f, 0.0625f), new Vector2(0.75f, 0.0625f) },
        // Crack #1
        { new Vector2(0.0f, 0.0f),  new Vector2(0.0625f, 0.0f), new Vector2(0.0f, 0.0625f), new Vector2(0.0625f, 0.0625f) },
        // Crack #2
        { new Vector2(0.0625f, 0.0f),  new Vector2(0.125f, 0.0f), new Vector2(0.0625f, 0.0625f), new Vector2(0.125f, 0.0625f) },
        // Crack #3
        { new Vector2(0.125f, 0.0f),  new Vector2(0.1875f, 0.0f), new Vector2(0.125f, 0.0625f), new Vector2(0.1875f, 0.0625f) },
        // Crack #4
        { new Vector2(0.1875f, 0.0f),  new Vector2(0.25f, 0.0f), new Vector2(0.1875f,0.0625f), new Vector2(0.25f, 0.0625f) },
        // Crack #5
        { new Vector2(0.25f, 0.0f),  new Vector2(0.3125f, 0.0f), new Vector2(0.25f, 0.0625f), new Vector2(0.3125f, 0.0625f) },
        // Crack #6
        { new Vector2(0.3125f, 0.0f),  new Vector2(0.375f, 0.0f), new Vector2(0.3125f, 0.0625f), new Vector2(0.375f, 0.0625f) },
        // Crack #7
        { new Vector2(0.375f, 0.0f),  new Vector2(0.4375f, 0.0f), new Vector2(0.375f, 0.0625f), new Vector2(0.4375f, 0.0625f) },
        // Crack #8
        { new Vector2(0.4375f, 0.0f),  new Vector2(0.5f, 0.0f), new Vector2(0.4375f, 0.0625f), new Vector2(0.5f, 0.0625f) },
        // Crack #9
        { new Vector2(0.5f, 0.0f),  new Vector2(0.5625f, 0.0f), new Vector2(0.5f, 0.0625f), new Vector2(0.5625f, 0.0625f) },
        // Crack #10
        { new Vector2(0.5625f, 0.0f),  new Vector2(0.625f, 0.0f), new Vector2(0.5625f, 0.0625f), new Vector2(0.625f, 0.0625f) }
    };

    int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1) i = World.chunkSize - 1;
        else if (i == World.chunkSize) i = 0;
        return i;
    }

    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunks;

        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Chunk nChunk;
            Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3((x - (int)pos.x) * World.chunkSize, (y - (int)pos.y) * World.chunkSize, (z - (int)pos.z) * World.chunkSize);
            string nName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            if (World.chunks.TryGetValue(nName, out nChunk)) chunks = nChunk.chunkData;
            else return false;
        }
        else chunks = owner.chunkData;

        try { return chunks[x, y, z].isSolid; }
        catch (System.IndexOutOfRangeException ex) { if (ex != null) Debug.Log("Index: " + x + "_" + y + "_" + z + " is empty"); }

        return false;
    }

    void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        List<Vector2> suvs = new List<Vector2>();
        int[] triangles = new int[6];

        // All possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (bType == BlockType.GRASS && side == CubeSide.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == CubeSide.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }
        // Set crack uvs
        suvs.Add(blockUVs[(int)(health + 1), 3]);
        suvs.Add(blockUVs[(int)(health + 1), 2]);
        suvs.Add(blockUVs[(int)(health + 1), 0]);
        suvs.Add(blockUVs[(int)(health + 1), 1]);

        // All possible vertices in a cube
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch(side)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;
        }

        uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        mesh.name = "ScriptedMesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.SetUVs(1, suvs);
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));

        quad.transform.parent = parent.transform;
        quad.transform.position = pos;
        meshFilter.mesh = mesh;
    }

    void SetType(BlockType b)
    {
        bType = b;
        if (bType == BlockType.AIR) isSolid = false;
        else isSolid = true;

        health = BlockType.NOCRACK;
        curHealth = blockHealthMax[(int)bType];
    }

    public void Reset()
    {
        health = BlockType.NOCRACK;
        curHealth = blockHealthMax[(int)bType];
        owner.Redraw();
    }

    public bool BuildBlock(BlockType b)
    {
        SetType(b);
        owner.Redraw();
        return true;
    }

    public bool HitBlock()
    {
        if (curHealth == -1) return false;

        curHealth--;
        health++;

        if (curHealth == (blockHealthMax[(int)bType] - 1))
            owner.mb.StartCoroutine(owner.mb.HealBlock(pos));

        if (curHealth <= 0)
        {
            bType = BlockType.AIR;
            isSolid = false;
            health = BlockType.NOCRACK;
            owner.Redraw();
            return true;
        }
        owner.Redraw();
        return false;
    }

    public void Draw()
    {
        if (bType == BlockType.AIR) return;

        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1))
            CreateQuad(CubeSide.FRONT);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1))
            CreateQuad(CubeSide.BACK);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z))
            CreateQuad(CubeSide.TOP);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z))
            CreateQuad(CubeSide.BOTTOM);
        if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z))
            CreateQuad(CubeSide.LEFT);
        if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z))
            CreateQuad(CubeSide.RIGHT);
    }

    public Block(BlockType b, Vector3 pos, GameObject p, Chunk c)
    {
        this.pos = pos;
        parent = p;
        owner = c;
        SetType(b);
    }
}
