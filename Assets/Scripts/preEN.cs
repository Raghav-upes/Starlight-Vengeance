using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class preEN : MonoBehaviour
{

    private Transform player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false; // Flag to track if the enemy is chasing the player

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isChasingPlayer)
        {

            // If chasing, set the destination to the player's position
            navMeshAgent.destination = movePositionTransform.position;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= 5.0f)
        {
            void Update()
            {
                isChasingPlayer = false;
                // Handle attack behavior (e.g., deal damage to player)
            }
        }
        else
        {
            void Update()
            {
                isChasingPlayer = true;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If the player enters the sphere collider, start chasing
            isChasingPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If the player exits the sphere collider, stop chasing
            isChasingPlayer = false;
        }
    }
}
