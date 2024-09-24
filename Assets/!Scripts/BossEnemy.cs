using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    Transform player;
    [SerializeField] private Transform mouthTransform;

    public float rotationSpeed = 5.0f;  // Speed of the Lerp rotation
    public float rotationThreshold = 15f;  // Maximum allowed angle (in degrees)
    private bool isThrowing = false;
    private GameObject laserShoot;
    [SerializeField] private float projectileSpeed = 10f;

    Animator anim;

    private float hp = 75f;

    [SerializeField] private float sandEffectSpeed = 5f;
    [SerializeField] private float sandEffectLifetime = 3f;
    private GameObject currentSandEffect;
    [SerializeField] private GameObject sandEffectPrefab;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        laserShoot = Resources.Load<GameObject>("Sphere");
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
        if (distanceToPlayer > 35f)
        {
            anim.ResetTrigger("tongueAttack");
            anim.ResetTrigger("groundAttack");
            anim.SetTrigger("laserAttack");
         /*   this.GetComponentInChildren<Weapon>().Beam();*/
         /*   StartCoroutine(stopbeam());*/
        }
        if(distanceToPlayer<35f && distanceToPlayer > 30f)
        {
            anim.ResetTrigger("tongueAttack");
            anim.ResetTrigger("laserAttack");
            anim.SetTrigger("groundAttack");
            InstantiateSandEffect();
        }
        if (distanceToPlayer<=30f)
        {
            anim.ResetTrigger("laserAttack");
            anim.ResetTrigger("groundAttack");
            anim.SetTrigger("tongueAttack");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    IEnumerator stopbeam()
    {
        yield return new WaitForSeconds(7f);
        this.GetComponentInChildren<Weapon>().StopBeam();
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
            StartCoroutine(MoveAndDestroySandEffect(currentSandEffect));
        }
        else
        {
            Debug.LogError("Sand effect prefab is not assigned.");
        }
    }

    IEnumerator MoveAndDestroySandEffect(GameObject sandEffect)
    {
        float elapsedTime = 0f;

        while (elapsedTime < sandEffectLifetime)
        {
            sandEffect.transform.Translate(Vector3.forward * sandEffectSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(sandEffect);
        currentSandEffect = null;
    }
    public void OnChildCollision(GameObject child, Collision collision)
    {
        Debug.LogWarning("Collision detected in child: " + child.name);

        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            if (child.CompareTag("centre"))
            {
                hp = hp - 15;
                anim.SetTrigger("groundAttack");
                StartCoroutine(stopGroundAttack());
            }
            else
            {
                hp = hp - 3;

            }
            if (hp > 0)
            {
                anim.SetTrigger("hitInsideMouth");
            }
            else if (hp <= 0)
            {
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
    //private IEnumerator shootLaser()
    //{
    //    isThrowing = true;
    //    yield return new WaitForSeconds(2f);

    //    if (laserShoot == null)
    //    {
    //        Debug.LogError("Projectile prefab is not assigned or not found in Resources folder.");
    //        yield return null;
    //    }
    //    GameObject laser = Instantiate(laserShoot, mouthTransform.position, mouthTransform.rotation);
    //    Rigidbody rb = laser.GetComponent<Rigidbody>();
    //    if (rb != null)
    //    {
    //        Vector3 direction = (Camera.main.transform.position - mouthTransform.position).normalized;
    //        rb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
    //    }
    //    yield return new WaitForSeconds(5f);
    //    isThrowing = false;
    //}
}
