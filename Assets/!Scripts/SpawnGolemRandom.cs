using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGolemRandom : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
    public float minSpawnDistance = 10f; // Minimum spawn distance from the player
    public float maxSpawnDistance = 20f; // Maximum spawn distance from the player
    public float groundOffset = -2f; // Offset to place enemy slightly under the ground
    public int spawnCount = 5; // Number of enemies to spawn
    public Transform spawnAround;
    public float spawnDelay = 2f;

    public int EnemyDeathCount;
    public GameObject orbSphere;
    private void Start()
    {
        EnemyDeathCount = spawnCount;
        StartCoroutine(SpawnEnemiesAroundPlayer());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            this.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(RiseGolem());
        }
    }


    IEnumerator RiseGolem()
    {
        for (int i = 1; i < this.transform.childCount ; i++)
        {
            this.transform.GetChild(i).GetComponent<SphereCollider>().enabled = true;
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    IEnumerator SpawnEnemiesAroundPlayer()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();

            // Initially set Y to a high value (above the ground)
            spawnPosition.y = 50f;

            // Instantiate enemy and handle spawning
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity,this.transform);
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            // Raycast down to find the ground and adjust enemy's position
            RaycastHit hit;

            while (true)
            {
                spawnPosition = GetSpawnPosition();
                if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
                {
                    // Check if we hit the ground
                    if (hit.collider.CompareTag("Ground"))
                    {
                        // Set the enemy's Y position to slightly below the ground
                        enemy.transform.position = new Vector3(spawnPosition.x, hit.point.y + groundOffset, spawnPosition.z);

                        break;
                    }
                }
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        // Calculate the X and Z positions around the player using polar coordinates
        spawnPosition = spawnAround.position + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

        return spawnPosition;
    }


    public void GolemKilled()
    {
        EnemyDeathCount--;
        if (EnemyDeathCount == 0)
        {
            GetComponentInChildren<PurifyTower>().canvasText.text = "Purify the tower";
            orbSphere.GetComponent<SphereCollider>().enabled = true;
        }
    }
}
