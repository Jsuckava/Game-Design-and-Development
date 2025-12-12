using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BahayKuboTracker : MonoBehaviour
{
    [Header("Material UI (Shows Team Total)")]
    public TextMeshProUGUI bambooText;
    public TextMeshProUGUI nipaText;
    public TextMeshProUGUI hardwoodText;
    public TextMeshProUGUI sawaliText;

    [Header("Combined Progress UI")]
    public TextMeshProUGUI totalProgressText;
    
    [Header("Build State (Shared House)")]
    public bool haligiBuilt = false;
    public bool sahigBuilt = false;
    public bool paderBuilt = false;
    public bool bubongBuilt = false;

    [Header("Part Costs")]
    public int haligi_bambooCost = 12;
    public int sahig_hardwoodCost = 14;
    
    public int pader_sawaliCost = 16;
    public int pader_bambooCost = 12; 

    public int bubong_nipaCost = 15;
    public int bubong_bambooCost = 5; 

    private TurnManager turnManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        RefreshUI();
    }
    public string DestroyPart(string partName)
    {
        string resultMsg = "";
        
        switch (partName)
        {
            case "Haligi":
                if (haligiBuilt) { haligiBuilt = false; resultMsg = "The Haligi collapsed!"; }
                break;
            case "Sahig":
                if (sahigBuilt) { sahigBuilt = false; resultMsg = "The Sahig was destroyed!"; }
                break;
            case "Pader":
                if (paderBuilt) { paderBuilt = false; resultMsg = "The Pader was blown away!"; }
                break;
            case "Bubong":
                if (bubongBuilt) { bubongBuilt = false; resultMsg = "The Bubong flew off!"; }
                break;
        }

        RefreshUI(); 
        return resultMsg;
    }


    private void RefreshUI()
    {
        if (turnManager == null) return;
        
        int teamBamboo = 0;
        int teamNipa = 0;
        int teamHardwood = 0;
        int teamSawali = 0;

        List<PlayerStats> players = turnManager.GetAllActivePlayers();
        if (players != null)
        {
            foreach (PlayerStats p in players)
            {
                if (p != null)
                {
                    teamBamboo += p.bamboo;
                    teamNipa += p.nipaLeaves;
                    teamHardwood += p.hardwood;
                    teamSawali += p.sawali;
                }
            }
        }

        if (bambooText != null) bambooText.text = "Total Bamboo: " + teamBamboo;
        if (nipaText != null) nipaText.text = "Total Nipa: " + teamNipa;
        if (hardwoodText != null) hardwoodText.text = "Total Hardwood: " + teamHardwood;
        if (sawaliText != null) sawaliText.text = "Total Sawali: " + teamSawali;
       
        if (totalProgressText != null)
        {
            string status = "Team Bahay Kubo Status:\n";
            status += haligiBuilt ? "[X] Haligi (Done)\n" : $"Haligi (Bamboo: {teamBamboo}/{haligi_bambooCost})\n";
            status += sahigBuilt ? "[X] Sahig (Done)\n" : $"Sahig (Hardwood: {teamHardwood}/{sahig_hardwoodCost})\n";
            status += paderBuilt ? "[X] Pader (Done)\n" : $"Pader (Sawali: {teamSawali}/{pader_sawaliCost} | Bamboo: {teamBamboo}/{pader_bambooCost})\n";
            status += bubongBuilt ? "[X] Bubong (Done)" : $"Bubong (Nipa: {teamNipa}/{bubong_nipaCost} | Bamboo: {teamBamboo}/{bubong_bambooCost})";
            totalProgressText.text = status;
        }
    }

    public void AddBamboo(int amount) { RefreshUI(); }
    public void AddNipa(int amount) { RefreshUI(); }
    public void AddHardwood(int amount) { RefreshUI(); }
    public void AddSawali(int amount) { RefreshUI(); }
    
    public string AttemptBuild(PlayerStats player)
    {
        RefreshUI();

        if (haligiBuilt && sahigBuilt && paderBuilt && bubongBuilt) return "Complete";

        if (!haligiBuilt)
        {
            if (player.bamboo >= haligi_bambooCost) 
            {
                player.ChangeBamboo(-haligi_bambooCost);
                haligiBuilt = true; 
                RefreshUI();
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
                RefreshUI();
                return "Sahig";
            }
            return "NotEnoughHardwood"; 
        }

        if (haligiBuilt && sahigBuilt && !paderBuilt)
        {
            if (player.sawali >= pader_sawaliCost && player.bamboo >= pader_bambooCost)
            {
                player.ChangeSawali(-pader_sawaliCost);
                player.ChangeBamboo(-pader_bambooCost);
                paderBuilt = true; 
                RefreshUI();
                return "Pader";
            }
            if (player.sawali < pader_sawaliCost) return "NotEnoughSawali";
            if (player.bamboo < pader_bambooCost) return "NotEnoughBambooForPader";
        }

        if (haligiBuilt && sahigBuilt && paderBuilt && !bubongBuilt)
        {
            if (player.nipaLeaves >= bubong_nipaCost && player.bamboo >= bubong_bambooCost)
            {
                player.ChangeNipa(-bubong_nipaCost);
                player.ChangeBamboo(-bubong_bambooCost);
                bubongBuilt = true; 
                RefreshUI();
                return "Bubong"; 
            }
            if (player.nipaLeaves < bubong_nipaCost) return "NotEnoughNipa";
            if (player.bamboo < bubong_bambooCost) return "NotEnoughBambooForBubong";
        }
        
        return "Complete";
    }
}