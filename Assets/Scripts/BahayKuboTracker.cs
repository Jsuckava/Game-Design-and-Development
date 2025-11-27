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

    void Start()
    {
        UpdateGlobalDisplay();
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
        if (haligiBuilt && sahigBuilt && paderBuilt && bubongBuilt)
        {
            return "Complete";
        }

        if (!haligiBuilt)
        {
            if (player.bamboo >= haligi_bambooCost) 
            {
                player.ChangeBamboo(-haligi_bambooCost);
                haligiBuilt = true;
                return "Haligi";
            }
            return "NotEnoughBamboo"; 
        }

        if (haligiBuilt && !sahigBuilt)
        {
            if (player.hardwood >= sahig_hardwoodCost)
            {
                player.ChangeHardwood(-sahig_hardwoodCost);
                sahigBuilt = true;
                return "Sahig";
            }
            return "NotEnoughHardwood"; 
        }

        if (haligiBuilt && sahigBuilt && !paderBuilt)
        {
            if (player.sawali >= pader_sawaliCost)
            {
                player.ChangeSawali(-pader_sawaliCost);
                paderBuilt = true;
                return "Pader";
            }
            return "NotEnoughSawali"; 
        }

        if (haligiBuilt && sahigBuilt && paderBuilt && !bubongBuilt)
        {
            if (player.nipaLeaves >= bubong_nipaCost)
            {
                player.ChangeNipa(-bubong_nipaCost);
                bubongBuilt = true;
                if (haligiBuilt && sahigBuilt && paderBuilt && bubongBuilt)
                {
                     return "Complete";
                }
                return "Bubong"; 
            }
            return "NotEnoughNipa"; 
        }
        
        return "Complete";
    }
}