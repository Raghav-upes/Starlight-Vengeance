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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            bool morechild = false;
            if (other.transform.childCount > 0)
            {
                Destroy(other.transform.GetChild(0).gameObject);
                morechild = true;
            }
            GameObject orb = Instantiate(this.gameObject, other.transform);
            if (orb.GetComponentInParent<PurifyTower>() != null)
            {
                orb.GetComponentInParent<PurifyTower>().purify();
            }
           orb.AddComponent<OrbShooter>();
            if (orb.GetComponentInParent<ShieldActiavte>() != null)
            {
                orb.GetComponent<OrbShooter>().ShieldTimeStart();

                if (!morechild)
                {
                    orb.GetComponentInParent<ShieldActiavte>().ActivateShield();
                }
                else
                {
                    orb.GetComponentInParent<ShieldActiavte>().DeactivateShield();
                }
            }
            orb.transform.localScale = other.GetComponent<OrbSizeDefine>().mySize;
            orb.transform.localPosition = Vector3.zero;
            this.GetComponentInParent<OrbFormation>().StopAnim(LeftHand);
            if(orb.GetComponentInParent<Weapon>()!=null)
            orb.GetComponentInParent<Weapon>().Reload();

            Destroy(orb.GetComponent<SphereCollider>());
            Destroy(orb.GetComponent<OrbMerger>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            LeftHand.SendHapticImpulse(0, 0.2f, 0.1f);
            RightHand.SendHapticImpulse(0, 0.2f, 0.1f);
        }
    }
}
