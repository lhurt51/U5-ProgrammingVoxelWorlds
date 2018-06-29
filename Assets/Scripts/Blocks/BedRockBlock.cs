using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedRockBlock : Block {

    public static Vector2[,] UVs =
    {
        /*SIDE*/
        { new Vector2(0.3125f, 0.8125f), new Vector2(0.375f, 0.8125f), new Vector2(0.3125f, 0.875f), new Vector2(0.375f, 0.875f) }
    };

    public static int maxHealth = -1;

    public BedRockBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.BEDROCK, pos, p, c)
    {
        blockUVs = UVs;
    }

}
