using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HandAnimator : MonoBehaviour
{
    [SerializeField] private InputActionProperty trigger;
    [SerializeField] private InputActionProperty grip;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>(); 
    }
    private void Update()
    {
        float triggervalue = trigger.action.ReadValue<float>();
        float gripvalue = grip.action.ReadValue<float>();

        anim.SetFloat("Trigger", triggervalue);
        anim.SetFloat("Grip", gripvalue);
    }
}
