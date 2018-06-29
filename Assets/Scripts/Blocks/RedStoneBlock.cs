using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedStoneBlock : Block {

    public static Vector2[,] UVs =
    {
        /*SIDE*/
        { new Vector2(0.1875f, 0.75f), new Vector2(0.25f, 0.75f), new Vector2 (0.1875f, 0.8125f), new Vector2(0.25f, 0.8125f) }
    };

    public static int maxHealth = 50;

    public RedStoneBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.REDSTONE, pos, p, c)
    {
        blockUVs = UVs;
    }
}
