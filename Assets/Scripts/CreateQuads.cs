using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour {

    public enum BlockType
    {
        GRASS,
        DIRT,
        STONE
    };

    public Material blockMat;
    public BlockType bType;

    enum CubeSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    Vector2[,] blockUVs =
    {
        // Grass top
        { new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f), new Vector2(0.125f, 0.4275f), new Vector2(0.1875f, 0.4375f) },
        // Grass side
        { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f) },
        // Dirt
        { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f) },
        // Stone
        { new Vector2(0.0f, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0.0f, 0.9375f), new Vector2(0.0625f, 0.9375f) }
    };

    void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
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
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        MeshRenderer renderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        quad.transform.parent = this.gameObject.transform;
        meshFilter.mesh = mesh;
        renderer.material = blockMat;
    }

    void CombineQuads()
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

        renderer.material = blockMat;

        // Delete all uncombined children
        foreach (Transform quad in this.transform) Destroy(quad.gameObject);
    }

    void CreateCube()
    {
        CreateQuad(CubeSide.FRONT);
        CreateQuad(CubeSide.BACK);
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.BOTTOM);
        CreateQuad(CubeSide.LEFT);
        CreateQuad(CubeSide.RIGHT);
        CombineQuads();
    }

	// Use this for initialization
	void Start () {
        CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
