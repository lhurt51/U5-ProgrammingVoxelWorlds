using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedStoneBlock : Block {

    public Vector2[,] myUVs =
    {
        /*SIDE*/
        { new Vector2(0.1875f, 0.75f), new Vector2(0.25f, 0.75f), new Vector2 (0.1875f, 0.8125f), new Vector2(0.25f, 0.8125f) }
    };

    public RedStoneBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.REDSTONE, pos, p, c)
    {
        blockUVs = myUVs;
        MaxHealth = 45;
    }
}
