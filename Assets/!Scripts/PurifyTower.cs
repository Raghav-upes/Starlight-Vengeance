using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifyTower : MonoBehaviour
{
    public GameObject eye;
    public Material pinkMat;

    public void purify()
    {
        if (eye != null && pinkMat != null)
        {
            Renderer eyeRenderer = eye.GetComponent<Renderer>();
            if (eyeRenderer != null)
            {
                Material[] materials = eyeRenderer.materials;
                if (materials.Length > 1)
                {
                    materials[0] = pinkMat;
                    eyeRenderer.materials = materials;
                }
            }
            else
            {
                Debug.LogError("Eye does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Eye or Pink material is missing.");
        }
    }
}
