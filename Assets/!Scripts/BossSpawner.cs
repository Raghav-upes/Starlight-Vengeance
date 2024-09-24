using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BossSpawner : MonoBehaviour
{
    public GameObject FinalBoss;
    public GameObject SpawnPortal;
    public GameObject Portal;
    private bool isAnimating;
    public Vector3 originalScale;
    public float animationDuration = 0.3f;
    void Start()
    {
      /*  Instantiate(FinalBoss, SpawnPortal.transform.position, Quaternion.identity);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(ActivatePortal());

        }
    }

    public IEnumerator ActivatePortal()
    {
        StartCoroutine(ScaleCanvas(Vector3.zero, originalScale));
        yield return new WaitForSeconds(2f);
        Instantiate(FinalBoss, SpawnPortal.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        StartCoroutine(ScaleCanvas(originalScale, Vector3.zero, () =>
        {
            Portal.gameObject.SetActive(false);
        }));
    }


    private System.Collections.IEnumerator ScaleCanvas(Vector3 from, Vector3 to, System.Action onComplete = null)
    {

        isAnimating = true;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            Portal.transform.localScale = Vector3.Lerp(from, to, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Portal.transform.localScale = to;
        isAnimating = false;
        onComplete?.Invoke();
    }

}
