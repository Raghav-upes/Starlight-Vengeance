using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGolem: MonoBehaviour
{

    private Transform player;
    private AudioSource audioSource;
    EnemyAI enemyAI;
    [Header("Audio Clips")]
    public AudioClip wakeClip;
    public AudioClip step1Clip;
    public AudioClip step2Clip;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false;
    public float distanceTriggerOutZone;
    float enemySpeed;
    Animator anim;
    
    float distanceToPlayer;
    private bool isThrowing = false;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemySpeed=navMeshAgent.speed;
        audioSource = gameObject.GetComponent<AudioSource>();
    }


    private void Update()
    {
   distanceToPlayer= Vector3.Distance(transform.position, player.position);
        if (navMeshAgent.enabled)
        {
            if (isChasingPlayer)
            {
                navMeshAgent.destination = player.position;
                navMeshAgent.isStopped = false;
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
            if (distanceToPlayer > distanceTriggerOutZone)
            {
                isChasingPlayer = false;
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

            StartCoroutine(WakeRandom());
        }
    }


    IEnumerator WakeRandom()
    {
        yield return new WaitForSeconds(Random.Range(0,7));

        isChasingPlayer = true;
        anim.ResetTrigger("Idle");
        if (!anim.GetBool("WakeUp"))
        {
            navMeshAgent.enabled = true;
            anim.SetBool("WakeUp", true);
            enemyAI.PlayAudio(wakeClip);
            navMeshAgent.speed = 0;
            StartCoroutine(DelayAnimRun());
        }
        else
        {
            anim.SetTrigger("Run");
            enemyAI.PlayAudio(step1Clip);
            enemyAI.PlayAudio(step2Clip);
        }



        this.GetComponent<SphereCollider>().radius = distanceTriggerOutZone;
    }

    IEnumerator DelayAnimRun()
    {
        yield return new WaitForSeconds(1.3f);
        anim.SetTrigger("Run");
        enemyAI.PlayAudio(step1Clip);
        enemyAI.PlayAudio(step2Clip);
        navMeshAgent.speed = enemySpeed;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            isChasingPlayer = false;
            anim.SetTrigger("Idle");
            anim.ResetTrigger("Run");
        }
    }
}
