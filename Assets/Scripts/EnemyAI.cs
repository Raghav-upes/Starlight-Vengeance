using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    private Transform player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false; // Flag to track if the enemy is chasing the player
    Animator anim;

    private GameObject sphere;
    [SerializeField] private Transform mouthTransform;
    [SerializeField] private float projectileSpeed = 10f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        sphere = Resources.Load<GameObject>("Sphere");
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (navMeshAgent.enabled)
        {
            if (isChasingPlayer)
            {

                // If chasing, set the destination to the player's position

                navMeshAgent.destination = player.position;
                navMeshAgent.isStopped = false;

                if (distanceToPlayer < 7f)
                {
                    Debug.Log("Spit triggered");
                    anim.SetTrigger("Spit");
                    Invoke("ShootProjectile", 4f);
                }
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
            if (distanceToPlayer > 55f)
            {
                isChasingPlayer = false;
                // Handle attack behavior (e.g., deal damage to player)

            }
            else
            {
                isChasingPlayer = true;

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TTT");
            // If the player enters the sphere collider, start chasing
            isChasingPlayer = true;
            anim.ResetTrigger("Idle");
            anim.SetTrigger("Run");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("CHCH");
            // If the player exits the sphere collider, stop chasing
            isChasingPlayer = false;
            anim.SetTrigger("Idle");
            anim.ResetTrigger("Run");

        }
    }
    private void ShootProjectile()
    {
        if (sphere == null)
        {
            Debug.LogError("Projectile prefab is not assigned or not found in Resources folder.");
            return;
        }

        GameObject spit = Instantiate(sphere, mouthTransform.position, mouthTransform.rotation);
        Rigidbody rb = spit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - mouthTransform.position).normalized;
            rb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
        }
    }
}
