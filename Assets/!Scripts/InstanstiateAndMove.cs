using UnityEngine;

public class InstantiateAndMove : MonoBehaviour
{
    public GameObject objectToSpawn;  // Prefab of the object to spawn
    public int numberOfObjects = 10;  // Number of objects to spawn
    public float spawnRadius = 20f;   // Radius at which to spawn objects
    public float moveSpeed = 5f;      // Speed at which objects move towards the camera

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Randomly position the object on the boundary of the defined radius
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = 0;  // Keep the objects on the ground (optional)
            Vector3 spawnPosition = randomDirection * spawnRadius;

            // Instantiate the object at the calculated position
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

            // Rotate the object to face the camera
            FaceCamera(spawnedObject);

            // Start moving the object towards the camera
            StartCoroutine(MoveTowardsCamera(spawnedObject));
        }
    }

    void FaceCamera(GameObject obj)
    {
        // Calculate the direction from the object to the camera
        Vector3 directionToCamera = (Camera.main.transform.position - obj.transform.position).normalized;

        // Adjust the direction so that it doesn't look upwards or downwards if you want a flat rotation
        directionToCamera.y = 0;

        // Rotate the object to face the camera
        obj.transform.rotation = Quaternion.LookRotation(directionToCamera);
    }

    System.Collections.IEnumerator MoveTowardsCamera(GameObject obj)
    {
        while (obj != null)
        {
            // Calculate direction towards the camera
            Vector3 direction = (Camera.main.transform.position - obj.transform.position).normalized;

            // Move the object towards the camera
            obj.transform.position += direction * moveSpeed * Time.deltaTime;

            // Continue to the next frame
            yield return null;
        }
    }
}
