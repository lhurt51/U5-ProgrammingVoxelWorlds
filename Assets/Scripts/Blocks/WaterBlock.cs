using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBlock : Block {

    public Vector2[,] myUVs =
    {
        /*SIDE*/
        { new Vector2(0.875f, 0.125f),  new Vector2(0.9375f, 0.125f), new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f) }
    };

    public WaterBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.WATER, pos, p, c)
    {
        blockUVs = myUVs;
        MaxHealth = 8;
    }
}
