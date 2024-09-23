using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    Transform player;


    public float rotationSpeed = 5.0f;  // Speed of the Lerp rotation

    public float rotationThreshold = 15f;  // Maximum allowed angle (in degrees)

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }



    void Update()
    {
        // Find the direction to the player
        Vector3 direction = player.position - transform.position;

        // Remove the y-axis difference to only rotate on the XZ plane (optional)
        direction.y = 0;

        // Calculate the angle between the enemy's forward direction and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, direction);

        // Check if the player is outside the threshold angle
        if (angleToPlayer > rotationThreshold)
        {
            // Create the rotation towards the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate the enemy towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
