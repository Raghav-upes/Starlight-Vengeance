using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthLook : MonoBehaviour
{
    private void Update()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Make the object fully rotate to look at the camera
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.position - transform.position);
        }
    }
}
