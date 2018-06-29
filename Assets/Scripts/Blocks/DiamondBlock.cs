using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondBlock : Block {

    public static Vector2[,] UVs =
    {
        /*SIDE*/
        { new Vector2(0.0f, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0.0f, 0.9375f), new Vector2(0.0625f, 0.9375f) }
    };

    public static int maxHealth = 60;

    public DiamondBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.DIAMOND, pos, p, c)
    {
        blockUVs = UVs;
    }
}
