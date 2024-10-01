using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool sequenceActive = false;
    //private GameObject laserShoot;
    [SerializeField] private float projectileSpeed = 10f;

    public AudioClip screamClip;//Added
    public AudioClip hurtClip;//Added
    public AudioClip flameClip;//Added
    public AudioClip groundClip;//Added
    private AudioSource audioSource;//Added

    private bool isCrater = false;

    private int numAttack = 0;

    Animator anim;

    private float hp = 100f;

    public ParticleSystem ps;

    public TerrainData originalLand;
    [SerializeField] private float sandEffectSpeed = 5f;
    [SerializeField] private float sandEffectLifetime = 3f;
    private GameObject currentSandEffect;
    private GameObject currentFlames;
    [SerializeField] private GameObject sandEffectPrefab;
    [SerializeField] private GameObject flamesEffectPrefab;

    public float craterRadius = 50f;   // Radius of the crater
    public float craterDepth = 0.2f;  // Depth of the crater

    /*private float[,] originalHeights;  // To store the original heightmap
    *//*private float[,] tempHeights;*//*      // Temporary heightmap for modifications
    private int heightMapWidth;
    private int heightMapHeight;*/

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

        //Terrain terrain = Terrain.activeTerrain;
        //if (terrain == null)
        //{
        //    Debug.LogError("No active terrain found in the scene.");
        //    return;
        //}

       /* TerrainData terrainData = terrain.terrainData;
        //terrain.terrainData = FlatLand;
        heightMapWidth = terrainData.heightmapResolution;
        heightMapHeight = terrainData.heightmapResolution;*/
/*
        // Store the original terrain heights once at the start of the game
        originalHeights = terrainData.GetHeights(0, 0, heightMapWidth, heightMapHeight);*/

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

        if (distanceToPlayer < 80f && distanceToPlayer > 30f)
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
        if (distanceToPlayer <= 30f)
        {
            anim.ResetTrigger("laserAttack");
            anim.ResetTrigger("groundAttack");
            anim.SetTrigger("tongueAttack");
            PlayAudio(screamClip, "Scream audio");
        }
    }
    IEnumerator TriggerGroundAttackSequence()
    {
        sequenceActive = true;
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
        if (other.CompareTag("Ground") && !isCrater)
        {
            
            isCrater = true;
            
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
            Instantiate(ps, transform.position, Quaternion.Euler(90, 0, 0));
            StartCoroutine(DisableGravityAfterDelay());
            this.GetComponent<BossEnemy>().enabled = true;
        }
    }
    private IEnumerator DisableGravityAfterDelay()
    {
        yield return new WaitForSeconds(0.13f);
        CreateCrater(transform.position);
        yield return new WaitForSeconds(2f);
        


        // Disable gravity and make the Rigidbody kinematic after the delay
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
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
            currentFlames = Instantiate(flamesEffectPrefab, mouthTransform.position, mouthTransform.rotation);
            StartCoroutine(ScaleOverTime(currentFlames.transform, 6f)); // 6 seconds duration for scaling
        }
        StartCoroutine(stopLaserAttack(currentFlames));
    }

    IEnumerator ScaleOverTime(Transform flameTransform, float duration)
    {
        float timeElapsed = 0f;
        Vector3 initialScale = flameTransform.localScale;
        Vector3 finalScale = new Vector3(1f, 1f, 1f);

        while (timeElapsed < duration)
        {
            flameTransform.localScale = Vector3.Lerp(initialScale, finalScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        flameTransform.localScale = finalScale;
    }

    IEnumerator stopLaserAttack(GameObject obj)
    {
        anim.ResetTrigger("laserAttack");
        yield return new WaitForSeconds(6f);
        isFlameThrowing = false;
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

    private void CreateCrater(Vector3 impactPosition)
    {
        Debug.LogError("adsa");
        Terrain.activeTerrain.terrainData = originalLand;
        /*// Access the terrain directly from the active terrain in the hierarchy
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain found in the scene.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;  // Access the TerrainData from the Terrain component
        int heightMapWidth = terrainData.heightmapResolution;
        int heightMapHeight = terrainData.heightmapResolution;

        // Convert world position to terrain position
        Vector3 terrainPosition = impactPosition - terrain.transform.position;

        // Convert to heightmap space
        float relativeX = terrainPosition.x / terrainData.size.x;
        float relativeZ = terrainPosition.z / terrainData.size.z;

        int craterPosX = Mathf.RoundToInt(relativeX * heightMapWidth);
        int craterPosZ = Mathf.RoundToInt(relativeZ * heightMapHeight);

        int craterWidth = Mathf.RoundToInt(craterRadius / terrainData.size.x * heightMapWidth);
        int craterHeight = Mathf.RoundToInt(craterRadius / terrainData.size.z * heightMapHeight);

        // Get the heightmap surrounding the impact point
        float[,] heights = terrainData.GetHeights(craterPosX - craterWidth / 2, craterPosZ - craterHeight / 2, craterWidth, craterHeight);

        *//*// Modify the heightmap to create a crater
        for (int x = 0; x < craterWidth; x++)
        {
            for (int z = 0; z < craterHeight; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(craterWidth / 2, craterHeight / 2));
                float heightAdjustment = Mathf.Max(0, (craterRadius - distance) / craterRadius) * craterDepth;
                heights[x, z] -= heightAdjustment;  // Lower the height to create a depression
            }
        }*//*

        for (int x = 0; x < craterWidth; x++)
        {
            for (int z = 0; z < craterHeight; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(craterWidth / 2, craterHeight / 2));
                if (distance < craterWidth / 2)
                {
                    float heightAdjustment = Mathf.Max(0, (craterRadius - distance) / craterRadius) * craterDepth;
                    heights[x, z] -= heightAdjustment;
                }
            }
        }

        // Apply the modified heightmap back to the terrain
        terrainData.SetHeights(craterPosX - craterWidth / 2, craterPosZ - craterHeight / 2, heights);*/
        Terrain.activeTerrain.GetComponent<TerrainCollider>().terrainData = originalLand;
        DestroyCraterObjects(impactPosition);
    }

    private void DestroyCraterObjects(Vector3 impactPosition)
    {
        GameObject[] craterObjects = GameObject.FindGameObjectsWithTag("CraterObjects");
        foreach (GameObject obj in craterObjects)
        {
            float distanceToCrater = Vector3.Distance(impactPosition, obj.transform.position);
            if (distanceToCrater <= craterRadius)
            {
                Destroy(obj);
            }
        }
    }

   /* public void ResetTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain found in the scene.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        terrainData.SetHeights(0, 0, originalHeights);
    }*/
}
