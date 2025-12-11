using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float pressScale = 0.9f;
    public float animeSpeed = 0.1f;
    private Vector3 originalScale;
    private Coroutine currentAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }
    // chineck ko lang kung nag cocommit ng maayos sa github.
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down on " + gameObject.name);
        if(currentAnim != null)
        {
            StopCoroutine(currentAnim);
        }
        currentAnim = StartCoroutine(AnimToScale(originalScale * pressScale));
        // transform.localScale = originalScale * 0.9f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(currentAnim != null)
        {
            StopCoroutine(currentAnim);
        }
        currentAnim = StartCoroutine(AnimToScale(originalScale));
        // transform.localScale = originalScale;
    }

    private System.Collections.IEnumerator AnimToScale(Vector3 targetScale)
    {
        Vector3 start = transform.localScale;
        float time = 0f;
        while (time < animeSpeed)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, targetScale, time / animeSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}