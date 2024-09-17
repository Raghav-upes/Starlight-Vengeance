using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbGollum : MonoBehaviour
{
    private EnemyGolem enemyGolem;

    private void Start()
    {
        enemyGolem = GetComponentInParent<EnemyGolem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enemyGolem != null)
        {
            enemyGolem.OnChildCollision(this.gameObject, collision);
        }
    }
}
