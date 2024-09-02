using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class OrbMerger : MonoBehaviour
{
    // Start is called before the first frame update
    List<InputDevice> leftHandDevices;
    List<InputDevice> rightHandDevices;

    InputDevice LeftHand;
    InputDevice RightHand;
    void Start()
    {
        leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (leftHandDevices.Count > 0)
        {
            LeftHand = leftHandDevices[0];
            RightHand = rightHandDevices[0];

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            if(other.transform.childCount>0)
                Destroy(other.transform.GetChild(0).gameObject);
           GameObject orb=Instantiate(this.gameObject,other.transform);
            orb.transform.localScale = Vector3.one;
            this.GetComponentInParent<OrbFormation>().StopAnim(LeftHand);
            orb.GetComponentInParent<Weapon>().Reload();
            orb.AddComponent<OrbShooter>();
            Destroy(orb.GetComponent<SphereCollider>());
            Destroy(orb.GetComponent<OrbMerger>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Gun"))
        {*/
            LeftHand.SendHapticImpulse(0, 0.2f, 0.1f);
            RightHand.SendHapticImpulse(0, 0.2f, 0.1f);
       /* }*/
    }
}
