using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroller : MonoBehaviour {

    private Vector2 uvSpeed = new Vector2(0.0075f, 0.01f);
    private Vector2 uvOffset = Vector2.zero;

    private void LateUpdate()
    {
        uvOffset += (uvSpeed * Time.deltaTime);

        // Ensure we dont scroll past the edge of our texture
        if (uvOffset.x > 0.0625f) uvOffset = new Vector2(0.0f, uvOffset.y);
        if (uvOffset.y > 0.0625f) uvOffset = new Vector2(uvOffset.x, 0.0f);

        this.GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex", uvOffset);
    }
}
