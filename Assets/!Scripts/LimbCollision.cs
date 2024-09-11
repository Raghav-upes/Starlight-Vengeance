using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    private CharacterFire characterFire;

    private void Start()
    {
        characterFire = GetComponentInParent<CharacterFire>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (characterFire != null)
        {
            characterFire.OnChildCollision(this.gameObject, collision);
        }
    }
}
