using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public GameObject creditsPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creditsPanel.gameObject.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.gameObject.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.gameObject.SetActive(false);
    }
}
