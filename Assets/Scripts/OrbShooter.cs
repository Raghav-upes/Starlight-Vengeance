using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbShooter : MonoBehaviour
{

    public void ChangeOrb(int currentAmmo,int ammoCapacity)
    {
        if(currentAmmo == 0)
        {
            Destroy(gameObject);
        }

        float scale=(float)currentAmmo/(float)ammoCapacity;
        this.transform.localScale=new Vector3(scale,scale,scale);

      
    }
}
