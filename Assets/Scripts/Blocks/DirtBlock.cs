using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtBlock : Block {

    public static Vector2[,] UVs =
    {
        /*SIDE*/
        { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f),new Vector2(0.1875f, 1.0f) }
    };

    public static int maxHealth = 20;

    public DirtBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.DIRT, pos, p, c)
    {
        blockUVs = UVs;
    }

}
