using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public List<GameObject> objectsToActivate;

/*    public List<GameObject> objectsToActivate2;*/
    public float delay = 1.5f;


    int i = 0;
    int j = 11;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        StartCoroutine(ActivateObjectsInSequence());
    }
    private void Start()
    {
        
    }

    private IEnumerator ActivateObjectsInSequence()
    {
        for ( i=0;i<j;i++ )
        {
            if (objectsToActivate[i])
            {
                objectsToActivate[i].GetComponent<AudioSource>().Play();
                objectsToActivate[i].GetComponent<SphereCollider>().enabled = true;
                objectsToActivate[i].GetComponent<OutFromtheSand>().enabled = true;
            }
            yield return new WaitForSeconds(delay);
        }
        StartCoroutine(ActivateObjects2());
    }

    IEnumerator ActivateObjects2()
    {
        for (int k = 23; k >= i; k--)
        {
            if (objectsToActivate[k])
            {
                objectsToActivate[k].GetComponent<AudioSource>().Play();
                objectsToActivate[k].GetComponent<SphereCollider>().enabled = true;
                objectsToActivate[k].GetComponent<OutFromtheSand>().enabled = true;
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
