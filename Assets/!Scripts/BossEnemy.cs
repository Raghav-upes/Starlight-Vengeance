using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    Transform player;
    [SerializeField] private Transform mouthTransform;

    public float rotationSpeed = 5.0f;  // Speed of the Lerp rotation
    public float rotationThreshold = 15f;  // Maximum allowed angle (in degrees)
    private bool isThrowing = false;
    private bool sequenceActive=false;
    //private GameObject laserShoot;
    [SerializeField] private float projectileSpeed = 10f;

    public AudioClip screamClip;//Added
    public AudioClip hurtClip;//Added
    public AudioClip flameClip;//Added
    public AudioClip groundClip;//Added
    private AudioSource audioSource;//Added

    private int numAttack=0;

    Animator anim;

    private float hp = 100f;

    private ParticleSystem ps;

    [SerializeField] private float sandEffectSpeed = 5f;
    [SerializeField] private float sandEffectLifetime = 3f;
    private GameObject currentSandEffect;
    private GameObject currentFlames;
    [SerializeField] private GameObject sandEffectPrefab;
    [SerializeField] private GameObject flamesEffectPrefab;

    bool isSandFormed = false;


    bool isFlameThrowing = false;
    private CapsuleCollider[] capsuleColliders;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (CapsuleCollider capsuleCollider in capsuleColliders)
        {
            capsuleCollider.enabled = true;
            capsuleCollider.gameObject.AddComponent<LimbBoss>();
        }

    }

    private void Awake()
    {

        audioSource = GetComponent<AudioSource>();//Added
        anim = GetComponent<Animator>();
        capsuleColliders = GetComponentsInChildren<CapsuleCollider>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 direction = player.position - transform.position;

        direction.y = 0;

        float angleToPlayer = Vector3.Angle(transform.forward, direction);

        if (angleToPlayer > rotationThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        if (distanceToPlayer<40f && distanceToPlayer > 30f)
        {
            if (!sequenceActive)
            {
                StartCoroutine(TriggerGroundAttackSequence());
                    if (!isSandFormed)
                    {
                        PlayAudio(groundClip, "Ground audio");
                        InstantiateSandEffect();
                    } 
            }
        }
        if (distanceToPlayer<=30f)
        {
            anim.ResetTrigger("laserAttack");
            anim.ResetTrigger("groundAttack");
            anim.SetTrigger("tongueAttack");
            PlayAudio(screamClip, "Scream audio");
        }
    }
    IEnumerator TriggerGroundAttackSequence()
    {
        sequenceActive=true;
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(DelayGroundAttacxk());
            anim.SetTrigger("groundAttack");
            PlayAudio(groundClip, "Ground audio");
            yield return new WaitForSeconds(2f);
        }
        StartCoroutine(TriggerLaserAttack());
    }
    IEnumerator TriggerLaserAttack()
    {
        anim.SetTrigger("laserAttack");
        yield return new WaitForSeconds(1f);
        if (!isFlameThrowing)
        {
            PlayAudio(flameClip, "Flame audio");
            startLaserAttack();
        }
        //yield return null; // Just to ensure the coroutine runs
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    IEnumerator DelayGroundAttacxk()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("groundAttack");
        anim.ResetTrigger("laserAttack");
    }
    void startLaserAttack()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (flamesEffectPrefab != null)
        {
            isFlameThrowing = true;
            currentFlames=Instantiate(flamesEffectPrefab, mouthTransform.position, Quaternion.LookRotation(direction));
        }
        StartCoroutine(stopLaserAttack(currentFlames));
    }
    IEnumerator stopLaserAttack(GameObject obj)
    {
        anim.ResetTrigger("laserAttack");
        yield return new WaitForSeconds(4f);
        isFlameThrowing=false;
        sequenceActive = false;
        Destroy(obj.gameObject);
    }
    IEnumerator stopGroundAttack()
    {
        yield return new WaitForSeconds(7f);
        anim.ResetTrigger("groundAttack");
    }

    void InstantiateSandEffect()
    {
        if (sandEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 2f;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward);
            currentSandEffect = Instantiate(sandEffectPrefab, spawnPosition, spawnRotation);
            isSandFormed = true;
            StartCoroutine(changeSnad(currentSandEffect));
        }
        else
        {
            Debug.LogError("Sand effect prefab is not assigned.");
        }
    }

    IEnumerator changeSnad(GameObject onj)
    {
        yield return new WaitForSeconds(4f);
        player.GetComponentInChildren<HealthCustom>().StartReducingHealthOverTime();
        yield return new WaitForSeconds(10f);
        isSandFormed = false;
        yield return new WaitForSeconds(5f);
        Destroy(onj.gameObject);
    }

    public void OnChildCollision(GameObject child, Collision collision)
    {
        Debug.LogWarning("Collision detected in child: " + child.name);

        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            if (child.CompareTag("centre"))
            {
                hp--;
                //Added
                PlayAudio(hurtClip, "Hurt audio");
                anim.SetTrigger("groundAttack");
                StartCoroutine(stopGroundAttack());
                Debug.LogError(hp);
            }
            else
            {
                hp = hp - 0.5f;
                Debug.LogError(hp);


            }
            if (hp > 0)
            {
                anim.SetTrigger("hitInsideMouth");
            }
            else if (hp <= 0)
            {
                anim.SetTrigger("Death");
                DestroyMe();
            }
        }
    }
    void DestroyMe()
    {
        this.GetComponent<BossEnemy>().enabled = false;
        this.GetComponentInChildren<LimbBoss>().enabled = false;
    }

    void PlayAudio(AudioClip clip, string clipName)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"{clipName} played.");
        }
    }


}
