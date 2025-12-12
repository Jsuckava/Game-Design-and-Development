using UnityEngine;
using TMPro;

public class BahayKuboTracker : MonoBehaviour
{
    public int TotalBamboo { get; private set; } = 0;
    public int TotalNipa { get; private set; } = 0;
    public int TotalHardwood { get; private set; } = 0;
    public int TotalSawali { get; private set; } = 0;
    
    [Header("Individual Material UI")]
    public TextMeshProUGUI bambooText;
    public TextMeshProUGUI nipaText;
    public TextMeshProUGUI hardwoodText;
    public TextMeshProUGUI sawaliText;

    [Header("Combined Progress UI")]
    public TextMeshProUGUI totalProgressText;
    
    [Header("Build State")]
    public bool haligiBuilt = false;
    public bool sahigBuilt = false;
    public bool paderBuilt = false;
    public bool bubongBuilt = false;

    [Header("Part Costs")]
    public int haligi_bambooCost = 16;
    public int sahig_hardwoodCost = 10;
    public int pader_sawaliCost = 12;
    public int bubong_nipaCost = 12;

    public AbilityManager abilityManager;
    private const float build_energy_cost = 10f;

    void Start()
    {
        UpdateGlobalDisplay();
        if (abilityManager == null) abilityManager = FindFirstObjectByType<AbilityManager>();
    }

    private void UpdateGlobalDisplay()
    {
        if (bambooText != null)
        {
            bambooText.text = "Bamboo: " + TotalBamboo;
        }
        if (nipaText != null)
        {
            nipaText.text = "Nipa: " + TotalNipa;
        }
        if (hardwoodText != null)
        {
            hardwoodText.text = "Hardwood: " + TotalHardwood;
        }
        if (sawaliText != null)
        {
            sawaliText.text = "Sawali: " + TotalSawali;
        }

        if (totalProgressText != null)
        {
            totalProgressText.text = $"Total Materials Collected:\n" +
                                     $"Bamboo: {TotalBamboo}\n" +
                                     $"Nipa: {TotalNipa}\n" +
                                     $"Hardwood: {TotalHardwood}\n" +
                                     $"Sawali: {TotalSawali}";
        }
    }

    public void AddBamboo(int amount)
    {
        TotalBamboo += amount;
        UpdateGlobalDisplay();
    }

    public void AddNipa(int amount)
    {
        TotalNipa += amount;
        UpdateGlobalDisplay();
    }

    public void AddHardwood(int amount)
    {
        TotalHardwood += amount;
        UpdateGlobalDisplay();
    }

    public void AddSawali(int amount)
    {
        TotalSawali += amount;
        UpdateGlobalDisplay();
    }
    
    public string AttemptBuild(PlayerStats player)
    {
        if (haligiBuilt && sahigBuilt && paderBuilt && bubongBuilt) return "Complete";

        // 1. CALCULATE ENERGY COST WITH DISCOUNT
        float finalEnergyCost = build_energy_cost;
        
        // Check for King Kiko's ability (Carpenter)
        if (abilityManager != null)
        {
            float multiplier = abilityManager.GetBuildEnergyCostReduction(player);
            finalEnergyCost *= multiplier; // If Carpenter, becomes 5.0f
        }

        // 2. CHECK IF PLAYER HAS ENOUGH ENERGY
        if (player.currentEnergy < finalEnergyCost)
        {
            return "NotEnoughEnergy"; 
        }

        // 3. ATTEMPT TO BUILD (Deduct Materials AND Energy)
        bool buildSuccess = false;
        string result = "";

        if (!haligiBuilt)
        {
            if (player.bamboo >= haligi_bambooCost) {
                player.ChangeBamboo(-haligi_bambooCost);
                haligiBuilt = true;
                result = "Haligi";
                buildSuccess = true;
            } else result = "NotEnoughBamboo";
        }
        else if (!sahigBuilt)
        {
            if (player.hardwood >= sahig_hardwoodCost) {
                player.ChangeHardwood(-sahig_hardwoodCost);
                sahigBuilt = true;
                result = "Sahig";
                buildSuccess = true;
            } else result = "NotEnoughHardwood";
        }
        else if (!paderBuilt)
        {
            if (player.sawali >= pader_sawaliCost) {
                player.ChangeSawali(-pader_sawaliCost);
                paderBuilt = true;
                result = "Pader";
                buildSuccess = true;
            } else result = "NotEnoughSawali";
        }
        else if (!bubongBuilt)
        {
            if (player.nipaLeaves >= bubong_nipaCost) {
                player.ChangeNipa(-bubong_nipaCost);
                bubongBuilt = true;
                result = "Bubong";
                buildSuccess = true;
            } else result = "NotEnoughNipa";
        }

        // 4. APPLY ENERGY COST IF SUCCESSFUL
        if (buildSuccess)
        {
            player.ChangeEnergy(-finalEnergyCost);
            Debug.Log($"Built {result}! Consumed {finalEnergyCost} Energy.");
        }

        return result;
    }
}