using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;
using Unity.Services.Analytics.Internal;

public class CharacterFire : MonoBehaviour
{

    public Material[] originalMaterials;  // Store the original materials
    private Material[] mat;  // Array of cloned materials
    public GameObject fireFX;
    public Light ExplodeLight;

    private bool startFade = false;  // For letting the script know it's time to start fading out
    private float t = 0.0f;
    private float fadeStart = 4;
    private float fadeEnd = 0;
    private float fadeTime = 1;
    private float pauseTime = 0;
    private float fadeSpeed = 0.05f;

    private float hp = 3f;




    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    private Rigidbody[] ragdollRigidbodies;
    private SphereCollider[] ragdollColliders;
    private CapsuleCollider[] ragdollCapsuleColliders;
    private BoxCollider[] ragdollBoxColliders;

    //public SkinnedMeshRenderer SMR;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<SphereCollider>();
        ragdollCapsuleColliders = GetComponentsInChildren<CapsuleCollider>();
        ragdollBoxColliders = GetComponentsInChildren<BoxCollider>();

    }

    void FaceCamera(GameObject obj)
    {
        // Calculate the direction from the object to the camera
        Vector3 directionToCamera = (Camera.main.transform.position - obj.transform.position).normalized;

        // Adjust the direction so that it doesn't look upwards or downwards if you want a flat rotation
        directionToCamera.y = 0;

        // Rotate the object to face the camera
        obj.transform.rotation = Quaternion.LookRotation(directionToCamera);
    }

    void Start()
    {

        // Clone the materials
        mat = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            mat[i] = new Material(originalMaterials[i]);  // Clone the material
        }

        foreach (CapsuleCollider capsuleCollider in ragdollCapsuleColliders)
        {
            capsuleCollider.enabled = false;
            capsuleCollider.gameObject.AddComponent<LimbCollision>();
        }


        foreach (SphereCollider collider in ragdollColliders)
        {
            collider.enabled = false;
            collider.gameObject.AddComponent<LimbCollision>();
        }
        this.GetComponent<SphereCollider>().enabled = true;
        // Assign the cloned materials to the game object
        GetComponentInChildren<Renderer>().materials = mat;

        // Reset all cloned materials to opaque untextured matte grey - good for testing purposes
        for (int rend = 0; rend <= mat.Length - 1; rend++)  // For each material
        {
            mat[rend].SetOverrideTag("RenderType", "");
            mat[rend].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat[rend].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat[rend].SetInt("_ZWrite", 1);
            mat[rend].DisableKeyword("_ALPHATEST_ON");
            mat[rend].DisableKeyword("_ALPHABLEND_ON");
            mat[rend].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat[rend].renderQueue = -1;
        }

        fireFX.SetActive(false);
    }

    public void OnChildCollision(GameObject child, Collision collision)
    {
        Debug.LogWarning("Collision detected in child: " + child.name);

        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            hp--;

            if (hp > 0)
            {
                // Trigger animations based on which child collided
                if (child.CompareTag("leftAnim"))
                {
                    anim.ResetTrigger("Idle");
                    anim.ResetTrigger("Run");
                    anim.SetTrigger("hitLeft");
                }
                else if (child.CompareTag("rightAnim"))
                {
                    anim.ResetTrigger("Idle");
                    anim.ResetTrigger("Run");
                    anim.SetTrigger("hitRight");
                }
                else if (child.CompareTag("centre"))
                {
                    anim.ResetTrigger("Idle");
                    anim.ResetTrigger("Run");
                    anim.SetTrigger("hitFront");
                }

                RagdollOn();
            }
            else if (hp == 0)
            {
                ResetMyMat();

                // Fade out the explosion light
                StartCoroutine(FadeLight());

                // Fade out the character
                StartCoroutine(FadeOut());

                foreach (CapsuleCollider cc in ragdollCapsuleColliders)
                {
                    cc.enabled = false;
                }
                foreach (SphereCollider collider in ragdollColliders)
                {
                    collider.enabled = false;
                }
                // Activate the fire effects
                fireFX.SetActive(true);
               /* this.gameObject.SetActive(true); // Hide the object after death*/
            }
        }
    }
    private void Update()
    {
        FaceCamera(this.gameObject);
    }

    void RagdollOn()
    {

        if (hp == 2)
        {
/*            foreach (CapsuleCollider cc in ragdollCapsuleColliders)
            {
                cc.enabled = false;
            }*/
        }

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Run");

        //foreach (Rigidbody rb in ragdollRigidbodies)
        //{
        //    //rb.isKinematic = false; 
        //    rb.detectCollisions = false;
        //    rb.useGravity = true;
        //    rb.freezeRotation = true;
        //}
        //foreach (SphereCollider col in ragdollColliders)
        //{
        //    col.enabled = false;
        //}
        navMeshAgent.speed = 0f;
        //navMeshAgent.isStopped = true;
        //if (anim != null)
        //{
        //    anim.enabled = false;
        //}
        StartCoroutine(disableRagdoll());
    }

    void RagdollOff()
    {

  /*      this.GetComponent<CapsuleCollider>().enabled = true;*/
     foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.freezeRotation = false;

        }
        /*    foreach (SphereCollider col in ragdollColliders)
         {
             col.enabled = true;
         }*/
        navMeshAgent.speed = 2.8f;
        //navMeshAgent.destination = movePositionTransform.position;
        //navMeshAgent.isStopped = false;
        if (anim != null)
        {
            anim.enabled = true;
        }

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("hitFront");
        anim.ResetTrigger("hitLeft");
        anim.ResetTrigger("hitRight");
        anim.SetTrigger("Run");
    }

    IEnumerator disableRagdoll()
    {
        yield return new WaitForSeconds(1f);
        RagdollOff();
    }

    private void ResetMyMat()
    {
        // Set all materials to Fade rendering mode so they can be faded out
        for (int setfade = 0; setfade < mat.Length; setfade++)
        {
            mat[setfade].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat[setfade].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat[setfade].EnableKeyword("_ALPHABLEND_ON");
            mat[setfade].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat[setfade].SetInt("_ZWrite", 0);
            mat[setfade].DisableKeyword("_ALPHATEST_ON");
            mat[setfade].renderQueue = 3000;
            mat[setfade].SetFloat("_Mode", 2);
        }
    }

    IEnumerator FadeOut()
    {
        bool isFading = true;
        while (isFading)
        {
            isFading = false;

            for (int rend = 0; rend <= mat.Length - 1; rend++)
            {
                Color color = mat[rend].color;  // The variable "color" is the renderer's material color
                if (color.a > 0)
                {  // If the color's alpha is greater than 0
                    color.a -= fadeSpeed;  // Decrease the color's alpha by fadeSpeed
                    if (color.a < 0) color.a = 0;  // Clamp alpha to zero
                    mat[rend].color = color;  // Update the renderer's material color
                    isFading = true;  // Continue the loop until all materials are fully faded
                }
            }

            yield return null;  // Wait for the next frame before continuing
        }

        StartCoroutine(DestroyMe());
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    IEnumerator FadeLight()
    {

        while (t < fadeTime)
        {
            if (pauseTime == 0)
            {
                t += Time.deltaTime;
            }

            ExplodeLight.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
            yield return 0;
        }

        t = 0;
    }
}