using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurifyTower : MonoBehaviour
{
    public GameObject eye;
    public Material greenMat;
    public Material redMat;
    public TextMeshProUGUI canvasText;
    public Canvas canvas;

    public GameObject ToBeNectTower;
    public GameObject NextGunToBeUnlock;
    public GameObject NextGunParentToBeUnlock;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the collider has the "Player" tag (which should be your XR rig)
        if (other.CompareTag("Player"))
        {
            // Change the text on the canvas when the player enters the trigger area
            if (canvasText != null)
            {
                canvasText.text = "Defeat All The Enemies!";
            }
            else
            {
                Debug.LogError("Canvas text (TextMeshPro) is not assigned.");
            }

            if (eye != null && redMat != null)
            {
                Renderer eyeRenderer = eye.GetComponent<Renderer>();
                if (eyeRenderer != null)
                {
                    Material[] materials = eyeRenderer.materials;
                    if (materials.Length > 1)
                    {
                        materials[0] = redMat;
                        eyeRenderer.materials = materials;
                    }
                }
                else
                {
                    Debug.LogError("Eye does not have a Renderer component.");
                }
            }
            else
            {
                Debug.LogError("Eye or Pink material is missing.");
            }
        }
    }
    public void purify()
    {
        if (eye != null && greenMat != null)
        {
            Renderer eyeRenderer = eye.GetComponent<Renderer>();
            if (eyeRenderer != null)
            {
                Material[] materials = eyeRenderer.materials;
                if (materials.Length > 1)
                {
                    materials[0] = greenMat;
                    eyeRenderer.materials = materials;
                    ToBeNectTower.GetComponent<BoxCollider>().enabled=true;
                    ToBeNectTower.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
                    if (NextGunParentToBeUnlock != null && NextGunParentToBeUnlock != null)
                    {
                        NextGunToBeUnlock.SetActive(true);
                        NextGunParentToBeUnlock.GetComponent<BoxCollider>().enabled = true;
                        NextGunParentToBeUnlock.GetComponent<WeaponSpawner>().enabled = true;
                    }
                    canvas.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Eye does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Eye or Pink material is missing.");
        }
    }


}
