using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using UnityEngine.InputSystem; 

public class IntroController : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject introPanel;
    public Image logoImage;
    
    [Header("Animation Settings")]
    public float fadeDuration = 3.0f;  
    public float holdDuration = 1.0f;  
    
    private Color startColor = new Color(0f, 0f, 0f, 1f); 
    private Color targetColor = new Color(181f/255f, 181f/255f, 181f/255f, 1f); 

    [Header("Audio")]
    public AudioSource backgroundMusic; 
    private bool introFinished = false;

    void Start()
    {
        if (introPanel != null) introPanel.SetActive(true);
        
        if (backgroundMusic != null) 
        {
            backgroundMusic.Stop(); 
            backgroundMusic.loop = true;
        }

        if (logoImage != null)
        {
            logoImage.color = startColor;
        }
        else
        {
            Debug.LogError("Logo Image is not assigned in the IntroController!");
        }

        StartCoroutine(RevealLogoSequence());
    }

    void Update()
    {
        bool inputDetected = false;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) 
            inputDetected = true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) 
            inputDetected = true;

        if (!introFinished && inputDetected)
        {
            StopAllCoroutines();
            FinishIntro();
        }
    }

    IEnumerator RevealLogoSequence()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration; 
            
            if (logoImage != null)
            {
                logoImage.color = Color.Lerp(startColor, targetColor, t);
            }
            
            yield return null;
        }

        if (logoImage != null) logoImage.color = targetColor;

        yield return new WaitForSeconds(holdDuration);

        FinishIntro();
    }

    void FinishIntro()
    {
        if (introFinished) return;
        introFinished = true;

        if (introPanel != null) introPanel.SetActive(false);

        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
        
        this.enabled = false;
    }
}