using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

    public GameObject cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(r, out hit, 10))
            {
                Vector3 hitBlock = hit.point - hit.normal / 2.0f;
                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                List<string> updates = new List<string>();
                float thisCX = hit.collider.transform.position.x;
                float thisCY = hit.collider.transform.position.y;
                float thisCZ = hit.collider.transform.position.z;

                updates.Add(hit.collider.gameObject.name);

                // Update neighbours?
                if (x == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX - World.chunkSize, thisCY, thisCZ)));
                if (x == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX + World.chunkSize, thisCY, thisCZ)));
                if (y == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY - World.chunkSize, thisCZ)));
                if (y == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY + World.chunkSize, thisCZ)));
                if (z == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ - World.chunkSize)));
                if (z == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ + World.chunkSize)));

                foreach(string cname in updates)
                {
                    Chunk c;

                    if (World.chunks.TryGetValue(cname, out c))
                    {
                        DestroyImmediate(c.chunk.GetComponent<MeshFilter>());
                        DestroyImmediate(c.chunk.GetComponent<MeshRenderer>());
                        DestroyImmediate(c.chunk.GetComponent<Collider>());
                        c.chunkData[x, y, z].SetType(Block.BlockType.AIR);
                        c.DrawChunk();
                    }
                }
            }
        }
    }

}
