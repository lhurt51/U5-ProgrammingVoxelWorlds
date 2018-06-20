using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

    public GameObject cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            // Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                Chunk hitc;

                if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitc)) return;

                Vector3 hitBlock = (Input.GetMouseButton(0)) ? hit.point - hit.normal / 2.0f : hit.point + hit.normal / 2.0f;
                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                bool update = false;
                update = (Input.GetMouseButton(0)) ? hitc.chunkData[x, y, z].HitBlock() : hitc.chunkData[x, y, z].BuildBlock(Block.BlockType.STONE);

                if (update)
                {
                    List<string> updates = new List<string>();
                    float thisCX = hitc.chunk.transform.position.x;
                    float thisCY = hitc.chunk.transform.position.y;
                    float thisCZ = hitc.chunk.transform.position.z;

                    // Update neighbours?
                    if (x == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX - World.chunkSize, thisCY, thisCZ)));
                    if (x == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX + World.chunkSize, thisCY, thisCZ)));
                    if (y == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY - World.chunkSize, thisCZ)));
                    if (y == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY + World.chunkSize, thisCZ)));
                    if (z == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ - World.chunkSize)));
                    if (z == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ + World.chunkSize)));

                    foreach (string cname in updates)
                    {
                        Chunk c;

                        if (World.chunks.TryGetValue(cname, out c)) c.Redraw();
                    }
                }
            }
        }
    }

}
