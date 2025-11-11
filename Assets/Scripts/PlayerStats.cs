using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float currentEnergy = 30f;
    private AbilityType activeAbility = AbilityType.None;
    private bool isAbilityUsed = false; 
    
    public TMPro.TextMeshProUGUI energyText; 

    public void SetAbility(AbilityType ability)
    {
        activeAbility = ability;
        isAbilityUsed = false; 
    }

    public AbilityType GetActiveAbility()
    {
        if (!isAbilityUsed)
        {
            return activeAbility;
        }
        return AbilityType.None; 
    }
    
    public void ConsumeAbility()
    {
        if (activeAbility != AbilityType.None)
        {
            isAbilityUsed = true;
            Debug.Log(gameObject.name + "'s special ability (" + activeAbility.ToString() + ") has been used up!");
        }
    }

    public void ChangeEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
            Debug.Log(gameObject.name + " is exhausted and cannot move!");
        }
        
        if (energyText != null)
        {
            energyText.text = "Energy: " + currentEnergy.ToString();
        }
        
        Debug.Log(gameObject.name + " energy changed by " + amount + ". New energy: " + currentEnergy);
    }

    void Start()
    {
        ChangeEnergy(0); 
    }
}