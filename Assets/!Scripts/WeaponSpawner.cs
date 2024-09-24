using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject Weapon;
    private bool isButtonPreviouslyPressed;
    private bool isCanvasActive;
    public float scale;
    void Start()
    {

        if (Weapon != null)
        {
            isCanvasActive = Weapon.gameObject.activeSelf;
        }
        isButtonPreviouslyPressed = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RightHand"))
        {
           Weapon.SetActive(true);
        }
    }*/

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RightHand"))
        {
            var rightHandDevices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

            if (rightHandDevices.Count > 0)
            {
                var rightHandDevice = rightHandDevices[0];

                // Check if the "A" button is pressed
                bool isButtonPressed;
                if (rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out isButtonPressed))
                {
                    if (isButtonPressed && !isButtonPreviouslyPressed)
                    {
                        XRDirectInteractor interactor = other.GetComponentInParent<XRDirectInteractor>();

                        // Check if the interactor is already holding an object
                        if (interactor != null && !interactor.hasSelection)
                        {
                            GameObject op = Instantiate(Weapon, other.transform.position, other.transform.parent.rotation);

                            op.GetComponent<XRGrabInteractable>().enabled = true;
                            op.GetComponent<Rigidbody>().useGravity = true;
                            op.GetComponent<Rigidbody>().isKinematic = false;

                            op.transform.localScale = new Vector3(scale, scale, scale);

                            // Perform SelectEnter only if the interactor is not holding an object
                            interactor.interactionManager.SelectEnter(interactor, op.GetComponent<XRGrabInteractable>());
                        }
                    }
                    isButtonPreviouslyPressed = isButtonPressed;
                }
            }
        }
    }


    private void ToggleWeapon()
    {
        if (Weapon != null)
        {
            isCanvasActive = !isCanvasActive;
            Weapon.gameObject.SetActive(isCanvasActive);
        }
    }
}
