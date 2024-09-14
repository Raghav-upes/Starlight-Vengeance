using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGolem: MonoBehaviour
{

    private Transform player;
/*    private AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip runClip;
    public AudioClip idleClip;
    public AudioClip spitClip;
    public AudioClip hitClip;*/
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false;
    public float distanceTriggerOutZone;
    float enemySpeed;
    // Flag to track if the enemy is chasing the player
    Animator anim;
    /*
        private GameObject sphere;
        [SerializeField] private Transform mouthTransform;
        [SerializeField] private float projectileSpeed = 10f;*/
    float distanceToPlayer;
    private bool isThrowing = false;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemySpeed=navMeshAgent.speed;
/*        audioSource = GetComponent<AudioSource>();

        sphere = Resources.Load<GameObject>("Sphere");*/
    }


    private void Update()
    {
   distanceToPlayer= Vector3.Distance(transform.position, player.position);
        if (navMeshAgent.enabled)
        {
            if (isChasingPlayer)
            {

                // If chasing, set the destination to the player's position

                navMeshAgent.destination = player.position;
                navMeshAgent.isStopped = false;

/*                if (distanceToPlayer < 7f)
                {
                    Debug.Log("Spit triggered");
                    anim.SetTrigger("Spit");

                    StopCoroutine(ShootProjectile());
                }

                if (distanceToPlayer > 10 && distanceToPlayer < 55 && !isThrowing)
                {
                    StartCoroutine(ShootProjectile());

                }*/
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
            if (distanceToPlayer > distanceTriggerOutZone)
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

            StartCoroutine(WakeRandom());

     /*       PlayAudio(runClip);*/
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
            navMeshAgent.speed = 0;
            StartCoroutine(DelayAnimRun());
        }
        else
        {
            anim.SetTrigger("Run");
        }



        this.GetComponent<SphereCollider>().radius = distanceTriggerOutZone;
    }

    IEnumerator DelayAnimRun()
    {
        yield return new WaitForSeconds(1.3f);
        anim.SetTrigger("Run");
        navMeshAgent.speed = enemySpeed;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            isChasingPlayer = false;
            anim.SetTrigger("Idle");
            anim.ResetTrigger("Run");
/*            PlayAudio(idleClip);*/

        }
    }
/*    private IEnumerator ShootProjectile()
    {
        isThrowing = true;
        yield return new WaitForSeconds(2f);

        if (sphere == null)
        {
            Debug.LogError("Projectile prefab is not assigned or not found in Resources folder.");
            yield return null;
        }
        PlayAudio(spitClip);
        GameObject spit = Instantiate(sphere, mouthTransform.position, mouthTransform.rotation);
        Rigidbody rb = spit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (Camera.main.transform.position - mouthTransform.position).normalized;
            rb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(5f);
        isThrowing = false;
    }
    public void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }*/

}
