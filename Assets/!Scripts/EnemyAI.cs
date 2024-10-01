using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    private Transform player;
    private AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip runClip;
    public AudioClip idleClip;
    public AudioClip spitClip;
    public AudioClip hitClip;

    private HealthCustom healthCustom;
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

    public bool isThrowing=false;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

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
                    anim.ResetTrigger("Spit");

                    StopCoroutine(ShootProjectile());
                }

                if (distanceToPlayer > 10 && distanceToPlayer < 55 && !isThrowing)
                {
                    anim.SetTrigger("Spit");

                    StartCoroutine(ShootProjectile());

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
            PlayAudio(runClip);
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
            PlayAudio(idleClip);

        }
    }
    private IEnumerator ShootProjectile()
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
            //healthCustom.updateSpitEFfect();
        }
        yield return new WaitForSeconds(5f);
        isThrowing = false;

        
    }
    //IEnumerator updateSpit()
    //{
    //    yield return new WaitForSeconds(1f);
    //    healthCustom.updateSpitEFfect();
    //    //float timer = Time.deltaTime;
    //    //float alpha = Mathf.Clamp(1.0f - timer / 100.0f, 0.0f, 1.0f);
    //    //if (isThrowing)
    //    //{ 
    //    //    spitEffect.color = new Color(color.r, color.g, color.b, alpha);
    //    //}
    //    //spitEffect.color = new Color(color.r, color.g, color.b, 0);
    //}
    public void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

}
