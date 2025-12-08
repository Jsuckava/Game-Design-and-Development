using UnityEngine;
using UnityEngine.UI;

public class PopOutUp : MonoBehaviour
{
    public GameObject panelObject; 
    public CanvasGroup canvasGroup; 

    private void Awake()
    {
        if (panelObject == null) panelObject = this.gameObject;
        if (canvasGroup == null) canvasGroup = panelObject.GetComponent<CanvasGroup>();
    }

    public void ShowPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; 
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    public void HidePanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }
}