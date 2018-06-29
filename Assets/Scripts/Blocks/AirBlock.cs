using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBlock : Block {

    public static Vector2[,] UVs =
    { 
        /*SIDE*/
        { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f)},
    };

    public static int maxHealth = 0;

    public AirBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.AIR, pos, p, c)
    {
        blockUVs = UVs;
    }

}
