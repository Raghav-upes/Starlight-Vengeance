using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("HideMe", 5f);
    }


   void HideMe()
    {
       Destroy(this.gameObject);
    }
    // Update is called once per frame
   
}
