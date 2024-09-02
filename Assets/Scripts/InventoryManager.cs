using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CanvasToggle : MonoBehaviour
{
    public Canvas targetCanvas;
    public float animationDuration = 0.3f;  // Duration of the scale animation
    public float zOffset = 2.0f;  // Distance in front of the camera
    private bool isCanvasActive;
    private bool isButtonPreviouslyPressed;
    private bool isAnimating;
    private Vector3 originalScale;

    void Start()
    {
        if (targetCanvas != null)
        {
            isCanvasActive = targetCanvas.gameObject.activeSelf;
            originalScale = targetCanvas.transform.localScale;

            if (isCanvasActive)
            {
                targetCanvas.transform.localScale = originalScale;
            }
            else
            {
                targetCanvas.transform.localScale = Vector3.zero;
            }
        }
        isButtonPreviouslyPressed = false;
        isAnimating = false;
    }

    void Update()
    {
        // Check if the right-hand controller is available
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count > 0)
        {
            var rightHandDevice = rightHandDevices[0];

            // Check if the "A" button is pressed
            bool isButtonPressed;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isButtonPressed))
            {
                if (isButtonPressed && !isButtonPreviouslyPressed && !isAnimating)
                {
                    ToggleCanvas();
                }
                isButtonPreviouslyPressed = isButtonPressed;
            }
        }
    }

    private void ToggleCanvas()
    {
        if (targetCanvas != null)
        {
            isCanvasActive = !isCanvasActive;

            if (isCanvasActive)
            {
                // Set the canvas to face the camera and position it correctly
                PositionCanvas();

                targetCanvas.gameObject.SetActive(true);
                StartCoroutine(ScaleCanvas(Vector3.zero, originalScale));
            }
            else
            {
                StartCoroutine(ScaleCanvas(originalScale, Vector3.zero, () =>
                {
                    targetCanvas.gameObject.SetActive(false);
                }));
            }
        }
    }

    private void PositionCanvas()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Set the canvas position to be in front of the camera at the specified Z offset
            targetCanvas.transform.position = mainCamera.transform.position + mainCamera.transform.forward * zOffset;

            // Make the canvas face the camera
            targetCanvas.transform.rotation = Quaternion.LookRotation(targetCanvas.transform.position - mainCamera.transform.position);
        }
    }

    private System.Collections.IEnumerator ScaleCanvas(Vector3 from, Vector3 to, System.Action onComplete = null)
    {
        isAnimating = true;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            targetCanvas.transform.localScale = Vector3.Lerp(from, to, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetCanvas.transform.localScale = to;
        isAnimating = false;
        onComplete?.Invoke();
    }
}
