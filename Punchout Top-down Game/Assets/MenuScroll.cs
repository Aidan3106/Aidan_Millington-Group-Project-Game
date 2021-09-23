using UnityEngine;
using System.Collections.Generic;

public class MenuScroll : MonoBehaviour
{
    void Update()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>(); //Gets the mesh render from unity

        Material mat = mr.material; //finds the material

        Vector2 offset = mat.mainTextureOffset; //uses vector2 to offset the texture, giving it a scrolling effect

        offset.y += Time.deltaTime / 10f; //slows the offset scrolling

        offset.x += Time.deltaTime / 10f; //slows the offset scrolling

        mat.mainTextureOffset = offset; //updates the offset
    }
}
