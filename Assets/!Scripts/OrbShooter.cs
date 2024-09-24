using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrbShooter : MonoBehaviour
{
    private bool isAnimating;
    public Vector3 originalScale;
    public float animationDuration = 0.3f;

    public void ChangeOrb(int currentAmmo,int ammoCapacity)
    {
        if(currentAmmo == 0)
        {
            Destroy(gameObject);
        }

        float scale=(float)currentAmmo*this.GetComponentInParent<OrbSizeDefine>().mySize.x/(float)ammoCapacity;
        this.transform.localScale=new Vector3(scale,scale,scale);

      
    }


    public void ChangeOrb()
    {

    }


    private System.Collections.IEnumerator ScaleCanvas(Vector3 from, Vector3 to, System.Action onComplete = null)
    {

        isAnimating = true;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            this.transform.localScale = Vector3.Lerp(from, to, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = to;
        isAnimating = false;
        onComplete?.Invoke();
    }
}
