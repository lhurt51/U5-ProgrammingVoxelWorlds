using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlock : Block {

    public Vector2[,] myUVs =
    {
        /*SIDE*/
        { new Vector2(0.125f, 0.875f), new Vector2(0.1875f,0.875f), new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f) }
    };

    public SandBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.SAND, pos, p, c)
    {
        blockUVs = myUVs;
        MaxHealth = 15;
    }
}
