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
/*    [SerializeField] private Transform movePositionTransform;*/
    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false; // Flag to track if the enemy is chasing the player
    Animator anim;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (navMeshAgent.enabled)
        {
            if (isChasingPlayer)
            {

                // If chasing, set the destination to the player's position
              
                navMeshAgent.destination = player.position;
                navMeshAgent.isStopped = false;
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > 55f)
            {

                void Update()
                {
                    isChasingPlayer = false;

                }
                // Handle attack behavior (e.g., deal damage to player)

            }
            else
            {

                void Update()
                {
                    isChasingPlayer = true;

                }


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

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < 7f )
            {
                anim.SetTrigger("Spit");
            }
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
}
