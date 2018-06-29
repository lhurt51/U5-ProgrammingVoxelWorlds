using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum BlockType
    {
        GRASS,
        DIRT,
        WATER,
        STONE,
        SAND,
        LEAVES,
        WOOD,
        WOODBASE,
        BEDROCK,
        REDSTONE,
        DIAMOND,
        AIR
    };

    public BlockType bType;
    public bool isSolid;

    public Chunk Owner
    {
        get { return owner; }
    }

    public Vector3 Pos
    {
        get { return pos; }
    }

    public int CurHealth
    {
        get { return curHealth; }
        set { curHealth = (int)Mathf.Clamp(0.0f, maxHealth, value); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        //protected set { maxHealth = value; curHealth = maxHealth; }
        protected set { maxHealth = (bType != BlockType.BEDROCK || bType != BlockType.WATER || bType != BlockType.AIR) ? (int)Mathf.Max(10.0f, value) : (bType == BlockType.BEDROCK) ? -1 : value; }
    }

    protected Vector2[,] blockUVs;

    private enum HealthType
    {
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
    };

    private enum CubeSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    private HealthType health;
    private int curHealth;
    private int maxHealth;

    private Chunk owner;
    private GameObject parent;
    private Vector3 pos;

    /* private Vector2[,] blockUVs =
    {
        // Grass top
        { new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f), new Vector2(0.125f, 0.4275f), new Vector2(0.1875f, 0.4375f) },
        // Grass side
        { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f) },
        // Dirt
        { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f) },
        // Water
        { new Vector2(0.875f, 0.125f),  new Vector2(0.9375f, 0.125f), new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f) },
        // Stone
        { new Vector2(0.0f, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0.0f, 0.9375f), new Vector2(0.0625f, 0.9375f) },
        // Sand
        { new Vector2(0.125f, 0.875f), new Vector2(0.1875f,0.875f), new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f) },
        // Leaves
        { new Vector2(0.0625f, 0.375f), new Vector2(0.125f, 0.375f), new Vector2(0.0625f, 0.4375f), new Vector2(0.125f, 0.4375f) },
        // Wood
        { new Vector2(0.375f, 0.625f), new Vector2(0.4375f, 0.625f), new Vector2(0.375f, 0.6875f), new Vector2(0.4375f, 0.6875f) },
        // Woodbase
        { new Vector2(0.375f, 0.625f), new Vector2(0.4375f, 0.625f), new Vector2(0.375f, 0.6875f), new Vector2(0.4375f, 0.6875f) },
        // Bedrock
        { new Vector2(0.3125f, 0.8125f), new Vector2(0.375f, 0.8125f), new Vector2(0.3125f, 0.875f), new Vector2(0.375f, 0.875f) },
		// Redstone
        { new Vector2(0.1875f, 0.75f), new Vector2(0.25f, 0.75f), new Vector2 (0.1875f, 0.8125f), new Vector2(0.25f, 0.8125f) },
        // Diamond
        { new Vector2(0.125f, 0.75f), new Vector2(0.1875f, 0.75f), new Vector2(0.125f, 0.8125f), new Vector2(0.1875f, 0.8125f) },
    }; */

    private Vector2[,] healthUVs =
    {
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

    private int ConvertBlockIndexToLocal(int i)
    {
        if (i <= -1) i += World.chunkSize;
        else if (i >= World.chunkSize) i -= World.chunkSize;
        return i;
    }

    private bool HasSolidNeighbour(int x, int y, int z)
    {
        try
        {
            Block b = GetBlock(x, y, z);
            if (b != null) return (b.isSolid || b.bType == bType);
        }
        catch (System.IndexOutOfRangeException ex) { if (ex != null) Debug.Log("Index: " + x + "_" + y + "_" + z + " is empty"); }

        if (bType == BlockType.WATER) return true;
        return false;
    }

    private void SetTypeData(BlockType type, bool isConstructor)
    {
        switch (type)
        {
            case BlockType.GRASS:
                if (!isConstructor) blockUVs = GrassBlock.UVs;
                MaxHealth = GrassBlock.maxHealth;
                break;
            case BlockType.DIRT:
                if (!isConstructor) blockUVs = DirtBlock.UVs;
                MaxHealth = DirtBlock.maxHealth;
                break;
            case BlockType.WATER:
                if (!isConstructor) blockUVs = WaterBlock.UVs;
                MaxHealth = WaterBlock.maxHealth;
                break;
            case BlockType.STONE:
                if (!isConstructor) blockUVs = StoneBlock.UVs;
                MaxHealth = StoneBlock.maxHealth;
                break;
            case BlockType.SAND:
                if (!isConstructor) blockUVs = SandBlock.UVs;
                MaxHealth = SandBlock.maxHealth;
                break;
            case BlockType.LEAVES:
                if (!isConstructor) blockUVs = TreeBlock.SetTreeData(TreeBlock.tType, true);
                MaxHealth = TreeBlock.maxHealth;
                break;
            case BlockType.WOOD:
                if (!isConstructor) blockUVs = TreeBlock.SetTreeData(TreeBlock.tType, false);
                MaxHealth = TreeBlock.maxHealth;
                break;
            case BlockType.WOODBASE:
                if (!isConstructor) blockUVs = TreeBlock.SetTreeData(TreeBlock.tType, false);
                MaxHealth = TreeBlock.maxHealth;
                break;
            case BlockType.BEDROCK:
                if (!isConstructor) blockUVs = BedRockBlock.UVs;
                MaxHealth = BedRockBlock.maxHealth;
                break;
            case BlockType.REDSTONE:
                if (!isConstructor) blockUVs = RedStoneBlock.UVs;
                MaxHealth = RedStoneBlock.maxHealth;
                break;
            case BlockType.DIAMOND:
                if (!isConstructor) blockUVs = DiamondBlock.UVs;
                MaxHealth = DiamondBlock.maxHealth;
                break;
            default:
                if (!isConstructor) blockUVs = AirBlock.UVs;
                MaxHealth = AirBlock.maxHealth;
                break;
        }
    }

    private void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        List<Vector2> suvs = new List<Vector2>();
        int[] triangles = new int[6];

        // All possible UVs
        Vector2 uv00 , uv10, uv01, uv11;

        if ((side == CubeSide.TOP && blockUVs.GetLength(0) >= 2) || (side == CubeSide.BOTTOM && blockUVs.GetLength(0) == 2))
        {
            uv00 = blockUVs[1, 0];
            uv10 = blockUVs[1, 1];
            uv01 = blockUVs[1, 2];
            uv11 = blockUVs[1, 3];
        }
        else if (side == CubeSide.BOTTOM && blockUVs.GetLength(0) == 3)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }

        // Set crack uvs
        suvs.Add(healthUVs[(int)health, 3]);
        suvs.Add(healthUVs[(int)health, 2]);
        suvs.Add(healthUVs[(int)health, 0]);
        suvs.Add(healthUVs[(int)health, 1]);

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

    public BlockType GetBlockType(int x, int y, int z)
    {
        Block b = GetBlock(x, y, z);

        if (b == null) return BlockType.AIR;
        else return b.bType;
    }

    public Block GetBlock(int x, int y, int z)
    {
        Block[,,] chunks;

        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Chunk nChunk;
            int newX = (x < 0 || x >= World.chunkSize) ? (int)((x < 0) ? (-World.chunkSize + 1 + x) / World.chunkSize : x / World.chunkSize) * World.chunkSize : 0;
            int newY = (y < 0 || y >= World.chunkSize) ? (int)((y < 0) ? (-World.chunkSize + 1 + y) / World.chunkSize : y / World.chunkSize) * World.chunkSize : 0;
            int newZ = (z < 0 || z >= World.chunkSize) ? (int)((z < 0) ? (-World.chunkSize + 1 + z) / World.chunkSize : z / World.chunkSize) * World.chunkSize : 0;
            Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3(newX, newY, newZ);
            string nName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            if (World.chunks.TryGetValue(nName, out nChunk)) chunks = nChunk.chunkData;
            else return null;
        }
        else chunks = owner.chunkData;

        return chunks[x, y, z];
    }

    public void Reset()
    {
        health = HealthType.NOCRACK;
        curHealth = maxHealth;
        owner.Redraw();
    }

    public void SetType(BlockType b, bool isConstructor)
    {
        bType = b;
        SetTypeData(b, isConstructor);
        isSolid = (bType == BlockType.AIR || bType == BlockType.WATER) ? false : true;
        parent = (bType == BlockType.WATER) ? owner.fluid.gameObject : owner.chunk.gameObject;
        health = HealthType.NOCRACK;
        curHealth = maxHealth;
    }

    public bool BuildBlock(BlockType b)
    {
        if (b == BlockType.WATER) World.Queue.Run(owner.mb.Flow(this, BlockType.WATER, maxHealth, 10));
        else if (b == BlockType.SAND) World.Queue.Run(owner.mb.Drop(this, BlockType.SAND, 20));
        else
        {
            SetType(b, false);
            owner.Redraw();
        }
        return true;
    }

    public bool HitBlock()
    {
        if (curHealth == -1) return false;

        health = (HealthType)Mathf.Abs(Utils.Map(0.0f, 10.0f, 0.0f, maxHealth, --curHealth) - 10);

        if (curHealth == (maxHealth - 1)) World.Queue.Run(owner.mb.HealBlock(pos));

        if (curHealth <= 0)
        {
            if (bType == BlockType.SAND && pos.y == World.chunkSize - 1) GetBlock((int)pos.x, (int)pos.y + 1, (int)pos.z).owner.UpdateChunk();
            SetType(BlockType.AIR, false);
            owner.Redraw();
            owner.UpdateChunk();
            return true;
        }
        owner.Redraw();
        return false;
    }

    public void Draw()
    {
        if (bType == BlockType.AIR) return;

        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1)) CreateQuad(CubeSide.FRONT);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1)) CreateQuad(CubeSide.BACK);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z)) CreateQuad(CubeSide.TOP);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z)) CreateQuad(CubeSide.BOTTOM);
        if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z)) CreateQuad(CubeSide.LEFT);
        if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z)) CreateQuad(CubeSide.RIGHT);
    }

    public Block(BlockType b, Vector3 pos, GameObject p, Chunk c)
    {
        this.pos = pos;
        parent = p;
        owner = c;
        SetType(b, true);
    }
}
