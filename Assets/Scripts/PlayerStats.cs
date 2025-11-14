using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public SpriteRenderer playerVisual; 
    public TextMeshProUGUI myStatDisplay; 
    private int playerID; 

    [Header("Player Info")]
    public string playerName;
    public Sprite characterSprite;
    public AbilityType ability;
    
    [Header("Morality")]
    public int morality = 0; 
    private int minMorality = 0;
    private int maxMorality = 50;
    
    [Header("Energy")]
    public float maxEnergy = 30f;
    public float currentEnergy;
    private float minEnergy = 0f;

    [Header("Resources")]
    public int bamboo = 0; 
    public int nipaLeaves = 0;
    public int hardwood = 0;
    public int sawali = 0;
    private int minMaterials = 0;

    public void Initialize(string name, int charID, float maxEnergyVal, AbilityType abilityType, Sprite sprite, int playerNum)
    {
        playerName = name;
        maxEnergy = maxEnergyVal;
        currentEnergy = maxEnergy; 
        ability = abilityType;
        characterSprite = sprite; 
        playerID = playerNum; 
        
        if (playerVisual != null)
        {
            playerVisual.sprite = characterSprite;
        }
        else
        {
            Debug.LogError("PlayerVisual SpriteRenderer is not assigned in the PlayerStats script on " + name);
        }
        
        UpdateStatDisplay(); 
    }

    public void UpdateStatDisplay()
    {
        if (myStatDisplay != null)
        {
            myStatDisplay.text = $"{playerID}: {playerName} - {ability}\n" +
                                 $"Energy: {currentEnergy} | Morality: {morality}\n" +
                                 $"Bamboo: {bamboo} | Nipa: {nipaLeaves} | Hardwood: {hardwood} | Sawali: {sawali}";
        }
    }

    public void ChangeEnergy(float amount) 
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, minEnergy, maxEnergy);
        Debug.Log(playerName + "'s new energy: " + currentEnergy);
        UpdateStatDisplay(); 
    }

    public void ChangeMorality(int amount)
    {
        morality += amount;
        morality = Mathf.Clamp(morality, minMorality, maxMorality);
        Debug.Log(playerName + "'s new morality: " + morality);
        UpdateStatDisplay(); 
    }

    public void ChangeBamboo(int amount)
    {
        bamboo += amount;
        bamboo = Mathf.Max(bamboo, minMaterials); 
        UpdateStatDisplay();
    }
    public void ChangeNipa(int amount)
    {
        nipaLeaves += amount;
        nipaLeaves = Mathf.Max(nipaLeaves, minMaterials); 
        UpdateStatDisplay();
    }
    public void ChangeHardwood(int amount)
    {
        hardwood += amount;
        hardwood = Mathf.Max(hardwood, minMaterials); 
        UpdateStatDisplay();
    }
    public void ChangeSawali(int amount)
    {
        sawali += amount;
        sawali = Mathf.Max(sawali, minMaterials); 
        UpdateStatDisplay();
    }

    public AbilityType CheckAbility()
    {
        return ability;
    }

    public void ConsumeAbility()
    {
        Debug.Log(playerName + " consumed their ability!");
    }
}