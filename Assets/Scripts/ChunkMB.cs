﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB : MonoBehaviour {

    Chunk owner;

    public ChunkMB() {}

    public void SetOwner(Chunk o)
    {
        owner = o;
        InvokeRepeating("SaveProgress", 10, 1000);
    }

	public IEnumerator HealBlock(Vector3 pos)
    {
        yield return new WaitForSeconds(5);

        if (owner.chunkData[(int)pos.x, (int)pos.y, (int)pos.z].bType != Block.BlockType.AIR)
            owner.chunkData[(int)pos.x, (int)pos.y, (int)pos.z].Reset();
    }

    public IEnumerator Flow(Block b, Block.BlockType bt, int strength, int maxSize)
    {
        // Reduce the strength of the fluid block with each new block created
        if (maxSize <= 0) yield break;
        if (b == null) yield break;
        if (strength <= 0) yield break;
        if (b.bType != Block.BlockType.AIR) yield break;

        b.SetType(bt, false);
        b.CurHealth = strength;
        b.Owner.Redraw();
        yield return new WaitForSeconds(1);

        int x = (int)b.Pos.x;
        int y = (int)b.Pos.y;
        int z = (int)b.Pos.z;

        // Flow down if air block is underneath
        Block below = b.GetBlock(x, y - 1, z);
        if (below != null && below.bType == Block.BlockType.AIR)
        {
            World.Queue.Run(Flow(b.GetBlock(x, y - 1, z), bt, strength, --maxSize));
            yield break;
        }
        // Flow outward
        else
        {
            --strength;
            --maxSize;

            // Flow left
            World.Queue.Run(Flow(b.GetBlock(x - 1, y, z), bt, strength, maxSize));
            yield return new WaitForSeconds(1);

            // Flow right
            World.Queue.Run(Flow(b.GetBlock(x + 1, y, z), bt, strength, maxSize));
            yield return new WaitForSeconds(1);

            // Flow forward
            World.Queue.Run(Flow(b.GetBlock(x, y, z + 1), bt, strength, maxSize));
            yield return new WaitForSeconds(1);

            // Flow backward
            World.Queue.Run(Flow(b.GetBlock(x, y, z - 1), bt, strength, maxSize));
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator Drop(Block b, Block.BlockType bt, int maxDrop)
    {
        Vector3 pos;
        Block thisB = b;
        Block prevB = null;

        for (int i = 0; i < maxDrop; i++)
        {
            Block.BlockType prevT = thisB.bType;

            if (prevT != bt) thisB.SetType(bt, false);
            if (prevB != null)
            {
                prevB.SetType(prevT, false);
                if (thisB.Owner != prevB.Owner)
                {
                    pos = prevB.Pos;
                    prevB.Owner.Redraw();
                    if (pos.x == 0) prevB.GetBlock((int)pos.x - 1, (int)pos.y, (int)pos.z).Owner.Redraw();
                    if (pos.x == World.chunkSize - 1) prevB.GetBlock((int)pos.x + 1, (int)pos.y, (int)pos.z).Owner.Redraw();
                    if (pos.z == 0) prevB.GetBlock((int)pos.x, (int)pos.y, (int)pos.z - 1).Owner.Redraw();
                    if (pos.z == World.chunkSize - 1) prevB.GetBlock((int)pos.x, (int)pos.y, (int)pos.z + 1).Owner.Redraw();
                }
            }

            pos = thisB.Pos;
            thisB.Owner.Redraw();
            if (pos.x == 0) thisB.GetBlock((int)pos.x - 1, (int)pos.y, (int)pos.z).Owner.Redraw();
            if (pos.x == World.chunkSize - 1) thisB.GetBlock((int)pos.x + 1, (int)pos.y, (int)pos.z).Owner.Redraw();
            if (pos.z == 0) thisB.GetBlock((int)pos.x, (int)pos.y, (int)pos.z - 1).Owner.Redraw();
            if (pos.z == World.chunkSize - 1) thisB.GetBlock((int)pos.x, (int)pos.y, (int)pos.z + 1).Owner.Redraw();
            prevB = thisB;

            yield return new WaitForSeconds(0.125f);
            pos = thisB.Pos;

            thisB = thisB.GetBlock((int)pos.x, (int)pos.y - 1, (int)pos.z);
            if (thisB.isSolid) yield break;
        }
    }

    void SaveProgress()
    {
        if (owner.changed)
        {
            // owner.Save();
            owner.changed = false;
        }
    }
}
