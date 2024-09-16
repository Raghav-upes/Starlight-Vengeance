using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbFormation : MonoBehaviour
{
    public GameObject targetObject;
    public float scaleUpTime = 0.5f;
    public float scaleDownTime = 0.5f;
    public float displacementMagnitude = 0.01f; // The amount of random displacement during scaling

    public Vector3 targetScale = new Vector3(0.04f, 0.04f, 0.04f);
    private Vector3 initialScale = Vector3.zero;
    private Vector3 initialPosition;
    public bool isHoldingButton = false;
    private float scaleUpSpeed;
    private float scaleDownSpeed;

    AudioSource audioSource;
    public AudioClip rechargeDown;
    public AudioClip powerUP;



    void Start()
    {
        // Ensure the target object is inactive and scaled to zero at the start
        audioSource = GetComponent<AudioSource>();
        targetObject.SetActive(false);
        initialPosition = targetObject.transform.localPosition;
        targetObject.transform.localScale = initialScale;
        scaleUpSpeed = 1f / scaleUpTime;
        scaleDownSpeed = 1f / scaleDownTime;
    }

    void Update()
    {
        // Check if the primary button is pressed
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            var leftHandDevice = leftHandDevices[0];
            if (leftHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isPressed))
            {
                if (isPressed && !isHoldingButton)
                {
                    isHoldingButton = true;
                    audioSource.clip = powerUP;
                    StopAllCoroutines(); // Stop any ongoing scale down coroutine
                    StartCoroutine(ScaleObject(targetScale, scaleUpSpeed, leftHandDevice, true));
                  
                }
                else if (!isPressed && isHoldingButton)
                {
                    isHoldingButton = false;
                    audioSource.clip = rechargeDown;
                    StopAllCoroutines(); // Stop any ongoing scale up coroutine
                    StartCoroutine(ScaleObject(initialScale, scaleDownSpeed, leftHandDevice, false, deactivateAfter: true));
                }
            }
        }
    }


    public void StopAnim(InputDevice leftHandDevice)
    {
        isHoldingButton = true;
        StopAllCoroutines(); // Stop any ongoing scale up coroutine
        StartCoroutine(ScaleObject(initialScale, scaleDownSpeed, leftHandDevice, false, deactivateAfter: true));
    }

    private IEnumerator ScaleObject(Vector3 target, float speed, InputDevice device, bool isScalingUp, bool deactivateAfter = false)
    {
        // Activate the object if scaling up
        audioSource.Play();
        if (isScalingUp)
        {
            targetObject.SetActive(true);

        }

        Vector3 currentScale = targetObject.transform.localScale;
        Vector3 currentPosition = targetObject.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < 1f / speed)
        {
            elapsedTime += Time.deltaTime;
            targetObject.transform.localScale = Vector3.Lerp(currentScale, target, elapsedTime * speed);

            // Add subtle random displacement during scaling
            Vector3 randomDisplacement = new Vector3(
                Random.Range(-displacementMagnitude, displacementMagnitude),
                0f,
                Random.Range(-displacementMagnitude, displacementMagnitude)
            );
            targetObject.transform.localPosition = currentPosition + randomDisplacement;

            // Provide haptic feedback during the scaling
            device.SendHapticImpulse(0, 0.2f, 0.1f); // You can adjust the intensity and duration as needed

            yield return null;
        }
        audioSource.Stop();
        targetObject.transform.localScale = target;

        // Reset position to initial after scaling completes
        targetObject.transform.localPosition = initialPosition;

        // Deactivate the object if scaling down
        if (deactivateAfter)
        {
            yield return new WaitForSeconds(0.5f);
            targetObject.SetActive(false);
        }
    }
}
