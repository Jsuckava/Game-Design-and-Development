using UnityEngine;
using TMPro;
using System.Collections.Generic; 

public class PlayerStats : MonoBehaviour
{
    public SpriteRenderer playerVisual; 
    public TextMeshProUGUI myStatDisplay; 
    private int playerID; 

    public string playerName;
    public Sprite characterSprite;
    public AbilityType ability;
    
    public int morality = 0; 
    private int minMorality = 0;
    private int maxMorality = 50;
    
    public float maxEnergy = 30f;
    public float currentEnergy;
    private float minEnergy = 0f;
    public int bamboo = 0; 
    public int nipaLeaves = 0;
    public int hardwood = 0;
    public int sawali = 0;
    private int minMaterials = 0;

    public Dictionary<string, int> cropInventory = new Dictionary<string, int>();

    private TurnManager turnManager;
    private PopupController popupController;
    private BahayKuboTracker bahayKuboTracker;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        popupController = FindFirstObjectByType<PopupController>();
        bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>(); 
    }

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
            
        }
        
        UpdateStatDisplay(); 
    }

    public void UpdateStatDisplay()
    {
        if (myStatDisplay != null)
        {
            myStatDisplay.text = $"{playerID}: {playerName} - {ability}\n" +
                                 $"Energy: {currentEnergy} | Morality: {morality}";
        }
    }

    public void ChangeEnergy(float amount) 
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, minEnergy, maxEnergy);
        
        if (currentEnergy <= 0)
        {
            if (turnManager != null)
            {
                turnManager.EliminatePlayer(this);
            }
        }
        UpdateStatDisplay(); 
    }

    public void ChangeMorality(int amount)
    {
        morality += amount;
        morality = Mathf.Clamp(morality, minMorality, maxMorality);
        UpdateStatDisplay(); 
    }
    public void ChangeCropCount(string itemType, int amount)
    {
        if (cropInventory.ContainsKey(itemType))
        {
            cropInventory[itemType] += amount;
            if (cropInventory[itemType] < 0)
            {
                cropInventory[itemType] = 0;
            }
            if (cropInventory[itemType] == 0)
            {
                cropInventory.Remove(itemType); 
            }
        }
        else if (amount > 0)
        {
            cropInventory.Add(itemType, amount);
        }
    }

    public void ChangeBamboo(int amount)
    {
        bamboo += amount;
        bamboo = Mathf.Max(bamboo, minMaterials); 
        
        if (bahayKuboTracker != null)
        {
            bahayKuboTracker.AddBamboo(amount);
        }
        
        UpdateStatDisplay();
    }
    public void ChangeNipa(int amount)
    {
        nipaLeaves += amount;
        nipaLeaves = Mathf.Max(nipaLeaves, minMaterials); 
        
        if (bahayKuboTracker != null)
        {
            bahayKuboTracker.AddNipa(amount);
        }

        UpdateStatDisplay();
    }
    public void ChangeHardwood(int amount)
    {
        hardwood += amount;
        hardwood = Mathf.Max(hardwood, minMaterials); 
        
        if (bahayKuboTracker != null)
        {
            bahayKuboTracker.AddHardwood(amount);
        }
        
        UpdateStatDisplay();
    }
    public void ChangeSawali(int amount)
    {
        sawali += amount;
        sawali = Mathf.Max(sawali, minMaterials); 
        
        if (bahayKuboTracker != null)
        {
            bahayKuboTracker.AddSawali(amount);
        }

        UpdateStatDisplay();
    }

    public AbilityType CheckAbility()
    {
        return ability;
    }

    public void ConsumeAbility()
    {
        
    }
}