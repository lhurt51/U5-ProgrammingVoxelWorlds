using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBlock : Block {

    public Vector2[,] myUVs =
    { 
        /*SIDE*/
        { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f)},
    };

    public AirBlock(Vector3 pos, GameObject p, Chunk c) : base(BlockType.AIR, pos, p, c)
    {
        blockUVs = myUVs;
        MaxHealth = 0;
    }

}
