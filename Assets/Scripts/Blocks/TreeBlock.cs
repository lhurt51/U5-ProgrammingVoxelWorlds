using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBlock : Block {

    public enum TreeType
    {
        PINE
    };

    private static Vector2[,,] WoodUVs =
    {
        // Pine tree
        {
            /*SIDE*/
            { new Vector2(0.25f, 0.875f),  new Vector2(0.3125f, 0.875f), new Vector2(0.25f, 0.9375f), new Vector2(0.3125f, 0.9375f) },
            /*TOP*/
            { new Vector2(0.3125f, 0.875f),  new Vector2(0.375f, 0.875f), new Vector2(0.3125f, 0.9375f), new Vector2(0.375f, 0.9375f) },
        }
    };

    private static Vector2[,,] LeaveUVs =
    {
        // Pine tree leaves
        {
            /*SIDE*/
            { new Vector2(0.25f, 0.75f),  new Vector2(0.3125f, 0.75f), new Vector2(0.25f, 0.8125f), new Vector2(0.3125f, 0.8125f) }
        }
    };

    public static TreeType tType = TreeType.PINE;
    public static int maxHealth;

    public static Vector2[,] SetTreeData(TreeType tt, bool isLeaves)
    {
        Vector2[,,] allUVs = (isLeaves) ? LeaveUVs : WoodUVs;

        tType = tt;
        Vector2[,] UVs = new Vector2[allUVs.GetLength(1), allUVs.GetLength(2)];
        for (int i = 0; i < allUVs.GetLength(1); i++)
        {
            for (int j = 0; j < allUVs.GetLength(2); j++)
            {
                UVs[i, j] = allUVs[(int)tType, i, j];
            }
        }
        maxHealth = (isLeaves) ? 15 : 40;
        return UVs;
    }

    public TreeBlock(TreeType tt, bool isLeaves, bool isBase, Vector3 pos, GameObject p, Chunk c) : base((isLeaves) ? BlockType.LEAVES : ((isBase) ? BlockType.WOODBASE : BlockType.WOOD), pos, p, c)
    {
        Vector2[,,] allUVs = (isLeaves) ? LeaveUVs : WoodUVs;

        tType = tt;
        blockUVs = new Vector2[allUVs.GetLength(1), allUVs.GetLength(2)];
        for (int i = 0; i < allUVs.GetLength(1); i++)
        {
            for (int j = 0; j < allUVs.GetLength(2); j++)
            {
                blockUVs[i, j] = allUVs[(int)tType, i, j];
            }
        }
        maxHealth = (isLeaves) ? 15 : 40;
    }

}
