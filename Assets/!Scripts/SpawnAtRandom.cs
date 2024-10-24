using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class SpawnAtRandom : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
     Transform player; // Player's transform
    public float minSpawnDistance = 10f; // Minimum spawn distance from the player
    public float maxSpawnDistance = 20f; // Maximum spawn distance from the player
    public float groundOffset = -2f; // Offset to place enemy slightly under the ground
    public int spawnCount = 5; // Number of enemies to spawn

    public float spawnDelay;

    public int EnemyDeathCount;
    public GameObject orbSphere;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SpawnEnemiesAroundPlayer());
            this.GetComponent<BoxCollider>().enabled= false;

        }
    }
    void Start()
    {
        EnemyDeathCount = spawnCount;
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }




    IEnumerator SpawnEnemiesAroundPlayer()
    {
         yield return new WaitForSeconds(3f);
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
            int io = 0;
            while (io<10)
            {
                 spawnPosition = GetSpawnPosition();
                if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
                {
                    Debug.LogWarning(hit.collider.name + "900990090909");
                    // Check if we hit the ground
                    if (!hit.collider.CompareTag("Untagged") && !hit.collider.CompareTag("Cliffs") && hit.collider.CompareTag("Ground") && hit.collider.GetComponentInChildren<NavMeshSurface>()!=null)
                    {

                        // Set the enemy's Y position to slightly below the ground
                        enemy.transform.position = new Vector3(spawnPosition.x, hit.point.y + groundOffset, spawnPosition.z);
                        enemy.GetComponent<SphereCollider>().enabled = true;
                        enemy.GetComponent<OutFromtheSand>().enabled = true;
                        break;
                    }
                }
                io++;
            }
            if(io==10)
            {
               StartCoroutine(enemy.GetComponent<CharacterFire>().DestroyMe());
            }
            yield return new WaitForSeconds(spawnDelay); // Small delay between spawns
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        // Calculate the X and Z positions around the player using polar coordinates
        spawnPosition = player.position + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

        return spawnPosition;
    }


    public void SpidersKilled()
    {
        EnemyDeathCount--;
        if(EnemyDeathCount==0)
        {
            if(GetComponentInChildren<PurifyTower>()!=null)
            GetComponentInChildren<PurifyTower>().canvasText.text = "Purify the tower";
            if (GetComponentInChildren<PurifyTowerFinalBoss>() != null)
            {
                GetComponentInChildren<PurifyTowerFinalBoss>().canvasText.text = "Purify the tower";
               /* GetComponent<PurifyTowerFinalBoss>().spawnFinalBoss();*/

            }
            orbSphere.GetComponent<SphereCollider>().enabled = true;
        }
    }
}
