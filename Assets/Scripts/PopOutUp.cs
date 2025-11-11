using UnityEngine;
using UnityEngine.UI;

public class PopOutUp : MonoBehaviour
{
    public GameObject panelObject; 

    public void ShowPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
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