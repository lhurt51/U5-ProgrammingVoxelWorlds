using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

    public GameObject cam;
    public Block.BlockType buildType = Block.BlockType.DIRT;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) buildType = Block.BlockType.DIRT;
        if (Input.GetKeyDown(KeyCode.Alpha2)) buildType = Block.BlockType.STONE;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            // Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                Chunk hitc;

                if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitc)) return;

                Vector3 hitBlock = (Input.GetMouseButton(0)) ? hit.point - hit.normal / 2.0f : hit.point + hit.normal / 2.0f;
                Block b = World.GetWorldBlock(hitBlock);
                bool update = (Input.GetMouseButton(0)) ? b.HitBlock() : b.BuildBlock(buildType);

                hitc = b.Owner;
                if (update)
                {
                    hitc.changed = true;
                    List<string> updates = new List<string>();
                    float thisCX = hitc.chunk.transform.position.x;
                    float thisCY = hitc.chunk.transform.position.y;
                    float thisCZ = hitc.chunk.transform.position.z;

                    // Update neighbours?
                    if (b.Pos.x == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX - World.chunkSize, thisCY, thisCZ)));
                    if (b.Pos.x == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX + World.chunkSize, thisCY, thisCZ)));
                    if (b.Pos.y == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY - World.chunkSize, thisCZ)));
                    if (b.Pos.y == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY + World.chunkSize, thisCZ)));
                    if (b.Pos.z == 0) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ - World.chunkSize)));
                    if (b.Pos.z == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisCX, thisCY, thisCZ + World.chunkSize)));

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
