using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject FinalBoss;
    public GameObject SpawnPortal;
    void Start()
    {
      /*  Instantiate(FinalBoss, SpawnPortal.transform.position, Quaternion.identity);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Instantiate(FinalBoss,SpawnPortal.transform.position,Quaternion.identity);
        }
    }


}
