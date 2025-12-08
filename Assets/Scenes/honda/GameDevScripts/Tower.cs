using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public static int towerHealth = 100;
    public TMP_Text towerHealthText;
    public Slider towerSlider;
    public int damagePerHit = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        towerHealthText.text = towerHealth.ToString() + "/100";
        towerSlider.value = towerHealth;

        if (towerHealth <= 0)
        {
            Debug.Log("Tower Destroyed!");
            Destroy(this.gameObject);
            towerSlider.gameObject.SetActive(false);
            Destroy(towerHealthText.gameObject);
            // Destroy(this.towerSlider);
        }
    }
}
