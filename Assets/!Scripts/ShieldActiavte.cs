using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShieldActiavte : MonoBehaviour
{
    public GameObject shield;
    private bool isAnimating;
    public Vector3 originalScale;
    public float animationDuration = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        /*originalScale = shield.transform.localScale;*/
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ActivateShieldop()
    {
      //this.GetComponentInChildren<SphereCollider>().enabled = false;
        StartCoroutine(ScaleCanvas(Vector3.zero, originalScale, () =>
        {
            shield.SetActive(true);
        }));
        this.GetComponentInChildren<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(7f);
      
        StartCoroutine(ScaleCanvas(originalScale, Vector3.zero));
    }
    public void ActivateShield()
    {
        StartCoroutine(ActivateShieldop());
    }


    public IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(7f);
        StartCoroutine(ScaleCanvas(originalScale, Vector3.zero));
    }
    private System.Collections.IEnumerator ScaleCanvas(Vector3 from, Vector3 to, System.Action onComplete = null)
    {

        isAnimating = true;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            shield.transform.localScale = Vector3.Lerp(from, to, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shield.transform.localScale = to;
        isAnimating = false;
        onComplete?.Invoke();
    }
}
