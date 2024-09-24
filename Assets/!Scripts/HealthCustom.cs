using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthCustom : MonoBehaviour
{
    public SpriteRenderer blood;
    public int health = 100;
    private Color color;
    private bool isRegenerating = false;

    public float rotationDuration = 1.0f;
    public float regenDuration = 10.0f;
    private Coroutine regenCoroutine;

    public GameObject[] healthBarCube; 

    private void Start()
    {
        color = blood.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("leftAnim") || other.CompareTag("rightAnim") || other.CompareTag("centre"))
        {
            health -= 10;
            UpdateHealthBar();
            UpdateBloodAlpha();

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                isRegenerating = false;
            }
        }
        if (other.CompareTag("GollumAttack"))
        {
            health -= 40;
            UpdateHealthBar();
            UpdateBloodAlpha();

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                isRegenerating = false;
            }
        }
        if (other.CompareTag("BossAttack"))
        {
            health -= 100;
            UpdateHealthBar();
            UpdateBloodAlpha();

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                isRegenerating = false;
            }
        }
        if (health <= 0)
        {
            this.GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(RotateParentOverTime());
        }
        else if (!isRegenerating)
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private void UpdateBloodAlpha()
    {
        float alpha = Mathf.Clamp(1.0f - health / 100.0f, 0.0f, 1.0f);
        blood.color = new Color(color.r, color.g, color.b, alpha);
    }

    private void UpdateHealthBar()
    {
      
        int activeCubes = health / 10;  

        for (int i = 0; i < healthBarCube.Length; i++)
        {
            if (i < activeCubes)
            {
                healthBarCube[i].SetActive(true);  
            }
            else
            {
                healthBarCube[i].SetActive(false);
            }
        }
    }

    private IEnumerator RotateParentOverTime()
    {
        Transform parentTransform = this.transform.parent;

        if (parentTransform != null)
        {
            Quaternion initialRotation = parentTransform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, -90);
            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / rotationDuration;
                parentTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

                yield return null;
            }

            parentTransform.rotation = targetRotation;
            GameObject.FindGameObjectWithTag("Locomotion").gameObject.SetActive(false);
            this.GetComponentInParent<CharacterController>().enabled = false;
        }
    }

    private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;
        float elapsedTime = 0f;
        int initialHealth = health;

        while (health < 100 && elapsedTime < regenDuration)
        {
            elapsedTime += Time.deltaTime;


            health = Mathf.Clamp(initialHealth + (int)((elapsedTime / regenDuration) * 100), 0, 100);
            UpdateHealthBar();  
            UpdateBloodAlpha();

            yield return null;
        }

        health = 100;
        UpdateHealthBar(); 
        UpdateBloodAlpha();
        isRegenerating = false;
    }
}
