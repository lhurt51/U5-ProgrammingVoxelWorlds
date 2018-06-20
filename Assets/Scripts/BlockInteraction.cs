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
            // Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                Chunk hitc;
                Vector3 hitBlock = hit.point - hit.normal / 2.0f;
                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                if (World.chunks.TryGetValue(hit.collider.gameObject.name, out hitc) && hitc.chunkData[x, y, z].HitBlock())
                {
                    List<string> updates = new List<string>();
                    float thisCX = hitc.chunk.transform.position.x;
                    float thisCY = hitc.chunk.transform.position.y;
                    float thisCZ = hitc.chunk.transform.position.z;

                    // updates.Add(hit.collider.gameObject.name);

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
