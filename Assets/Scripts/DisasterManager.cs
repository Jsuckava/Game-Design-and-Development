using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DisasterManager : MonoBehaviour
{
    public TextMeshProUGUI diceResultText;
    public List<PlayerStats> allActivePlayers = new List<PlayerStats>();

    private const int GM_BASE_ROLL = 10;
    
    public int globalMorale = 0;
    private const int MAX_GLOBAL_MORALE = 100;

    public int RollD20()
    {
        int result = UnityEngine.Random.Range(1, 21);

        if (diceResultText != null)
        {
            diceResultText.text = "D20: " + result.ToString();
        }

        Debug.Log("D20 Roll Result: " + result);
        return result;
    }

    public void TriggerDisaster(string eventName, PlayerStats currentPlayer)
    {
        int playerRoll = RollD20();
        bool isImmune = (playerRoll > GM_BASE_ROLL);

        Debug.Log($"Disaster '{eventName}' triggered. Player Roll: {playerRoll}. Immune: {isImmune}");

        ResolveDisasterEffect(eventName, currentPlayer, isImmune);
    }

    private void ResolveDisasterEffect(string eventName, PlayerStats targetPlayer, bool isImmune)
    {
        if (isImmune)
        {
            Debug.Log($"{targetPlayer.gameObject.name} is immune to {eventName}!");
            return;
        }

        switch (eventName)
        {
            case "El Niño":
            case "Tsunami":
            case "Tornado":
            case "Landslide":
            case "Earthquake":
                ApplyGlobalEnergyLoss(-5f, "Disaster");
                break;

            case "Flood":
            case "Trash":
            case "Typhoon":
            case "Volcanic Eruption":
                ApplyGlobalEnergyLoss(-3f, "Disaster");
                break;

            case "Tuberculosis":
                ApplyTuberculosisDebuff(targetPlayer, -3f);
                break;

            case "Dengue":
            case "Leptospirosis":
            case "Hypertension":
                ApplyEnergyLoss(-3f, targetPlayer);
                break;

            default:
                Debug.LogWarning("Unknown disaster event: " + eventName);
                break;
        }
    }

    private void ApplyEnergyLoss(float amount, PlayerStats targetPlayer)
    {
        targetPlayer.ChangeEnergy(amount);
        Debug.Log(targetPlayer.gameObject.name + " lost " + (-amount) + " energy.");
    }

    public void ApplyGlobalEnergyLoss(float amount, string source)
    {
        if (allActivePlayers == null || allActivePlayers.Count == 0) return;

        List<PlayerStats> playersToDamage = new List<PlayerStats>(allActivePlayers);

        foreach (PlayerStats player in playersToDamage)
        {
            if (allActivePlayers.Contains(player))
            {
                player.ChangeEnergy(amount);
                Debug.Log(player.gameObject.name + " lost " + (-amount) + " energy due to " + source + ".");
            }
        }
    }

    public void ApplyTuberculosisDebuff(PlayerStats targetPlayer, float amount)
    {
        targetPlayer.ChangeEnergy(amount);

        int currentIndex = allActivePlayers.IndexOf(targetPlayer);
        if (currentIndex != -1)
        {
            int nextIndex = (currentIndex + 1) % allActivePlayers.Count;
            PlayerStats nextPlayer = allActivePlayers[nextIndex];

            if (nextPlayer != null)
            {
                nextPlayer.ChangeEnergy(amount);
                Debug.Log($"{nextPlayer.gameObject.name} also lost {(-amount)} energy due to proximity.");
            }
        }
    }
    
    public void AddGlobalMorale(int amount)
    {
        globalMorale += amount;
        globalMorale = Mathf.Clamp(globalMorale, 0, MAX_GLOBAL_MORALE);
        
        Debug.Log($"Global Morale increased by {amount}. Current Morale: {globalMorale}/{MAX_GLOBAL_MORALE}");

        if (globalMorale >= MAX_GLOBAL_MORALE)
        {
            TriggerGlobalMoraleReward();
        }
    }
    
    private void TriggerGlobalMoraleReward()
    {
        Debug.Log("GLOBAL MORALE REWARD UNLOCKED! Distributing benefits.");
        List<PlayerStats> playersToHeal = new List<PlayerStats>(allActivePlayers);

        foreach (PlayerStats player in playersToHeal)
        {
            player.ChangeEnergy(5f); 
        }
        
        globalMorale = 0;
    }
}