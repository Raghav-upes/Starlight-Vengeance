using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrbShooter : MonoBehaviour
{
    private bool isAnimating;
    public Vector3 originalScale;
    private float animationDuration = 7f;
    GameObject Player;

    private void Start()
    {

    }

    public void ChangeOrb(int currentAmmo,int ammoCapacity)
    {
        if(currentAmmo == 0)
        {
            Destroy(gameObject);
        }

        float scale=(float)currentAmmo*this.GetComponentInParent<OrbSizeDefine>().mySize.x/(float)ammoCapacity;
        this.transform.localScale=new Vector3(scale,scale,scale);

      
    }


    public void ShieldTimeStart()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        originalScale = new Vector3(3.6f, 3.6f, 3.6f);
        Player.GetComponentInChildren<HealthCustom>().StopReducingHealthOverTime();
        Player.GetComponentInChildren<HealthCustom>().enabled = false;
        Player.GetComponentInChildren<CapsuleCollider>().enabled = false;
        StartCoroutine(ScaleCanvas(originalScale, Vector3.zero));
        
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
        Player.GetComponentInChildren<HealthCustom>().enabled = true;
        Player.GetComponentInChildren<CapsuleCollider>().enabled = true;
        onComplete?.Invoke();
    }
}
