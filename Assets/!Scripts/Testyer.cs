using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testyer : MonoBehaviour
{
    Weapon op;
    void Start()
    {
        op=this.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        

       if(Input.GetKey(KeyCode.Space))
            op.RemoteFire();

        

    }
}
