using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB : MonoBehaviour {

    Chunk owner;

    public ChunkMB() { }

    public void SetOwner(Chunk o) { owner = o; }

	public IEnumerator HealBlock(Vector3 pos)
    {
        yield return new WaitForSeconds(3);

        if (owner.chunkData[(int)pos.x, (int)pos.y, (int)pos.z].bType != Block.BlockType.AIR)
            owner.chunkData[(int)pos.x, (int)pos.y, (int)pos.z].Reset();
    }
}
