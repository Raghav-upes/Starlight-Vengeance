using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGolem : MonoBehaviour
{
    private Transform player;
    private AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip wakeClip;
    public AudioClip step1Clip;
    public AudioClip step2Clip;
    public AudioClip hitclip;

    private float hp = 6f;

    private SphereCollider[] ragdollColliders;
    private CapsuleCollider[] ragdollCapsuleColliders;
    private BoxCollider[] ragdollBoxColliders;

    private NavMeshAgent navMeshAgent;
    private bool isChasingPlayer = false;
    public float distanceTriggerOutZone;
    float enemySpeed;
    Animator anim;

    float distanceToPlayer;
    private bool isThrowing = false;
    private bool isRunning = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemySpeed = navMeshAgent.speed;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on the GameObject.");
        }

        // Retrieve all child colliders
        ragdollColliders = GetComponentsInChildren<SphereCollider>();
        ragdollCapsuleColliders = GetComponentsInChildren<CapsuleCollider>();
        ragdollBoxColliders = GetComponentsInChildren<BoxCollider>();
    }

    public void enableColliders()
    {
        this.GetComponentInChildren<SphereCollider>().enabled = true;
        this.GetComponentInChildren<CapsuleCollider>().enabled = true;
        this.GetComponentInChildren<BoxCollider>().enabled = true;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        // Add LimbGollum to the colliders
        foreach (CapsuleCollider capsuleCollider in ragdollCapsuleColliders)
        {
            capsuleCollider.enabled = false;
            capsuleCollider.gameObject.AddComponent<LimbGollum>();
        }

        foreach (SphereCollider collider in ragdollColliders)
        {
            collider.enabled = false;
            collider.gameObject.AddComponent<LimbGollum>();
        }

        foreach (BoxCollider boxCollider in ragdollBoxColliders)
        {
            boxCollider.enabled = false;
            boxCollider.gameObject.AddComponent<LimbGollum>();
        }
    }

    private void Update()
    {
        if (player == null) return;

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
                    PlayAudio(hitclip);
                    StopCoroutine(runAgain());
                    StopRunningSounds();
                }
                else
                {
                    StartCoroutine(runAgain());

                    if (!isRunning)
                    {
                        StartRunningSounds();
                    }
                }
            }
            else
            {
                navMeshAgent.isStopped = true;
                StopRunningSounds();
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

    private void StartRunningSounds()
    {
        isRunning = true;
        StartCoroutine(PlayStepSounds());
    }

    private void StopRunningSounds()
    {
        isRunning = false;
        StopCoroutine(PlayStepSounds());
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WakeRandom());
        }
    }

    public IEnumerator WakeRandom()
    {
        yield return new WaitForSeconds(Random.Range(0, 7));

        isChasingPlayer = true;
        anim.ResetTrigger("Idle");

        if (!anim.GetBool("WakeUp"))
        {
            navMeshAgent.enabled = true;
            anim.SetBool("WakeUp", true);
            PlayAudio(wakeClip);
            navMeshAgent.speed = 0;
            StartCoroutine(DelayAnimRun());
        }
        else
        {
            anim.SetTrigger("Run");
            StartCoroutine(PlayStepSounds());
        }

        GetComponent<SphereCollider>().radius = distanceTriggerOutZone;
    }

    IEnumerator DelayAnimRun()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("Run");
        StartCoroutine(PlayStepSounds());
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
            Debug.Log("Playing audio: " + clip.name);
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Audio clip is null or AudioSource is missing.");
        }
    }

    IEnumerator PlayStepSounds()
    {
        while (isRunning)
        {
            PlayAudio(step1Clip);
            yield return new WaitForSeconds(step1Clip.length);
            PlayAudio(step2Clip);
            yield return new WaitForSeconds(step2Clip.length);
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
                anim.ResetTrigger("Run");
                anim.SetTrigger("hitGolum");
                StartCoroutine(runAgain());
            }
            else if (hp <= 0)
            {
                navMeshAgent.enabled = false;
                anim.SetTrigger("Death");
                StartCoroutine(DestroyMe());
            }
        }
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    IEnumerator runAgain()
    {
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("hitGolum");
        anim.SetTrigger("Run");
    }
}
