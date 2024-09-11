using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class DisableGun : MonoBehaviour
{
    // Start is called before the first frame update
    XRGrabInteractable xRGrabInteractable;
    void Start()
    {
        xRGrabInteractable=GetComponent<XRGrabInteractable>();
        xRGrabInteractable.selectExited.AddListener(onRelease);
    }

   void onRelease(SelectExitEventArgs args)
    {
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
