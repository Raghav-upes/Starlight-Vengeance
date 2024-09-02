using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCustom : MonoBehaviour
{
 public int health=100;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            health -= 10;
        }
    }
}
