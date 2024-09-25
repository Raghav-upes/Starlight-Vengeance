using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class EnemyGolem : MonoBehaviour
{

    private Transform player;
    private AudioSource audioSource;
    [Header("Audio Clips")]
    public AudioClip wakeClip;
    public AudioClip step1Clip;
    public AudioClip step2Clip;

    private float hp = 6f;

    private SphereCollider[] ragdollColliders;
    private CapsuleCollider[] ragdollCapsuleColliders;
    private BoxCollider[] ragdollBoxColliders;

    /*    bool isOkay = false;
    */
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (CapsuleCollider capsuleCollider in ragdollCapsuleColliders)
        {
            capsuleCollider.gameObject.AddComponent<LimbGollum>();
        }


        foreach (SphereCollider collider in ragdollColliders)
        {
            collider.gameObject.AddComponent<LimbGollum>();
        }

        foreach (BoxCollider boxcollider in ragdollBoxColliders)
        {
            boxcollider.gameObject.AddComponent<LimbGollum>();
        }
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
        enemySpeed = navMeshAgent.speed;
        audioSource = gameObject.GetComponent<AudioSource>();

        ragdollColliders = GetComponentsInChildren<SphereCollider>();
        ragdollCapsuleColliders = GetComponentsInChildren<CapsuleCollider>();
        ragdollBoxColliders = GetComponentsInChildren<BoxCollider>();
    }


    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (navMeshAgent.enabled)
        {
            if (isChasingPlayer)
            {
                navMeshAgent.destination = player.position;
                navMeshAgent.isStopped = false;

                if (distanceToPlayer < 5f)
                {
                    anim.SetTrigger("attackPlayer");
                    StopCoroutine(runAgain());
                }
                else
                {
                    StartCoroutine(runAgain());
                }
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

    /*    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //        StartCoroutine(WakeRandom());//
                WakeRandom();
            }
        }*/

    /*    private void OnTriggerStay(Collider other)
        {
            if (!isOkay && other.CompareTag("Player") )
            {
                isOkay = true;
                StartCoroutine(WakeRandom());
            }
        }*/

    public void WakeRandom()
    {
        /*        yield return new WaitForSeconds(Random.Range(0,7));
        */
        Debug.Log(gameObject.name);

        isChasingPlayer = true;
        anim.ResetTrigger("Idle");
        if (!anim.GetBool("WakeUp"))
        {
            navMeshAgent.enabled = true;
            anim.SetBool("WakeUp", true);
            PlayAudio(wakeClip);
            navMeshAgent.isStopped = true;
            StartCoroutine(DelayAnimRun());
        }
        else
        {
            anim.SetTrigger("Run");
            PlayAudio(step1Clip);
            PlayAudio(step2Clip);
        }



        this.GetComponent<SphereCollider>().radius = distanceTriggerOutZone;
    }

    IEnumerator DelayAnimRun()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("Run");
        PlayAudio(step1Clip);
        PlayAudio(step2Clip);
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


    public void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void OnChildCollision(GameObject child, Collision collision)
    {
        Debug.LogWarning("Collision detected in child: " + child.name);
        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            hp--;

            if (hp > 0)
            {
                if (child.CompareTag("GollumAttack"))
                {
                    anim.ResetTrigger("Run");
                    anim.SetTrigger("hitGolum");
                    anim.ResetTrigger("attackPlayer");
                    StartCoroutine(runAgain());
                }
                //else if (child.CompareTag("rightAnim"))
                //{
                //    anim.ResetTrigger("Run");
                //    anim.ResetTrigger("attackPlayer");
                //    anim.SetTrigger("hitGolum");
                //    StartCoroutine(runAgain());
                //}
                //else if (child.CompareTag("centre"))
                //{
                //    anim.ResetTrigger("Run");
                //    anim.ResetTrigger("attackPlayer");
                //    anim.SetTrigger("hitGolum");
                //    StartCoroutine(runAgain());
                //}
            }
            else if (hp == 0)
            {
                navMeshAgent.speed = 0;

                anim.SetTrigger("Death");
                anim.ResetTrigger("attackPlayer");
                anim.ResetTrigger("Run");
                anim.ResetTrigger("Idle");

                StartCoroutine(DestroyMe());
            }
        }
    }
    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(5f);
        GetComponentInParent<SpawnGolemRandom>().GolemKilled();
        gameObject.SetActive(false);
    }
    IEnumerator runAgain()
    {
        yield return new WaitForSeconds(1.5f);
        anim.ResetTrigger("hitGolum");
        anim.SetTrigger("Run");
    }
}