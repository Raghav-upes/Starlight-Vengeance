using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation

    void Update()
    {
        // Rotate the object around its local Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
