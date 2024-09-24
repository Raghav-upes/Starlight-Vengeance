using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbBoss : MonoBehaviour
{
    // Start is called before the first frame update
    private BossEnemy bossEnemy;

    private void Start()
    {
        bossEnemy = GetComponentInParent<BossEnemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bossEnemy != null)
        {
            bossEnemy.OnChildCollision(this.gameObject, collision);
        }
    }
}
