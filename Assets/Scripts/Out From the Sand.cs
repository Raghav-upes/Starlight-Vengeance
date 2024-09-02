using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class OutFromtheSand : MonoBehaviour
{
    public float jumpForce = 10f;  // Adjust the force of the jump
    public GameObject potholePrefab;  // Assign the pothole prefab in the Inspector

    private Rigidbody rb;
    private UnityEngine.AI.NavMeshAgent agent;

    bool flag = true;

    public float raycastDistance = 10f;

    GameObject op;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Disable the NavMeshAgent initially to allow the jump
        this.GetComponent<EnemyAI>().enabled = false;
        agent.enabled = false;



        Debug.Log(rb);

    }


    private void Update()
    {
        RaycastHit hit;
        // Cast a ray downward to find the ground position
        Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
        if (flag && Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Ground"))
            {
                flag = false;
                // Instantiate the pothole at the hit point
                GameObject chako=Instantiate(potholePrefab, new Vector3(hit.point.x,hit.point.y+0.2f,hit.point.z), Quaternion.identity);
                StartCoroutine(chakoOP(chako));

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && flag)
        {
          

            // Apply a jump force to the object
            if(rb==null)
                rb = GetComponent<Rigidbody>();

            rb.useGravity = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
           /* flag = false;*/


            /*  this.GetComponent<Animator>().SetTrigger("Jump");*/
            /*     this.GetComponent<CapsuleCollider>().enabled = true;*/
            // Invoke the PlacePothole function after a short delay
            Invoke("enableColl", 1f);
            Invoke("PlacePothole", 2.2f);
        }


    }

   
    IEnumerator chakoOP(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f);
        obj.GetComponentInChildren<Rigidbody>().isKinematic = true;
    }


    void enableColl()
    {
     
        this.GetComponent<CapsuleCollider>().enabled = true;
    }
    void PlacePothole()
    {
        // Place the pothole at the object's current position


        rb.isKinematic = true;
        this.GetComponent<EnemyAI>().enabled = true;
        // Re-enable the NavMeshAgent after the jump
        agent.enabled = true;
        this.enabled = false;
    }
}
