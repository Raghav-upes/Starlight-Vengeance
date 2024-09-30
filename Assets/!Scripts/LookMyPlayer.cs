using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LookMyPlayer : MonoBehaviour
{
   public bool isFlatDirection;

    public float rotationOffsetY;


    // Update is called once per frame
    void Update()
    {
        FaceCamera(this.gameObject);
    }

    void FaceCamera(GameObject obj)
    {
        // Calculate the direction from the object to the camera
        Vector3 directionToCamera = (Camera.main.transform.position - obj.transform.position).normalized;

        // Adjust the direction so that it doesn't look upwards or downwards if you want a flat rotation
        if (isFlatDirection)
        {
            directionToCamera.y+= rotationOffsetY;
/*            directionToCamera = Quaternion.Euler(0, 180, 0) * directionToCamera;*/
        }
        else
        {
            directionToCamera.y = 0;
        }


        // Rotate the object to face the camera
        obj.transform.rotation = Quaternion.LookRotation(directionToCamera);
    }
}
