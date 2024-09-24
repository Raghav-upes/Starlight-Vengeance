using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    private void Update()
    {
        this.transform.position = Camera.main.transform.parent.position;
    }
}
