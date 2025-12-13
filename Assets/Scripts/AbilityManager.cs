using System.Collections;
using TMPro;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public TextMeshProUGUI announceText;

    void Start()
    {
    }

    private void ShowAnnouncement(string message)
    {
        if (announceText == null) return;
        announceText.text = message;
        announceText.enabled = true;
        StopAllCoroutines();
        StartCoroutine(HideTextRoutine());
    }

    IEnumerator HideTextRoutine()
    {
        yield return new WaitForSeconds(5f);

        if (announceText != null)
        {
            announceText.enabled = false;
            announceText.text = "";
        }
    }
    
    public void CheckMinigameBonus(PlayerStats player, string minigameName)
    {
        if (player.ability == AbilityType.GeniusStudent && minigameName == "Bugtong")
        {
            float bonus = 5f;
            player.ChangeEnergy(bonus);
            string message = $"{player.playerName} answered riddle correctly! +{bonus} energy.";
            ShowAnnouncement(message);
        }
    }

    public void CheckCardObtainedBonus(PlayerStats player, string cardType)
    {
        if (player.ability == AbilityType.VegetableVendor && cardType == "Vegetable")
        {
            float bonus = 10f;
            player.ChangeEnergy(bonus);
            string message = $"{player.playerName} got vegetable! passive activated +{bonus} energy.";
            ShowAnnouncement(message);
        }

        if (player.ability == AbilityType.FruitVendor && cardType == "Fruit")
        {
            float bonus = 10f;
            player.ChangeEnergy(bonus);
            string message = $"{player.playerName} got fruit! passive activated +{bonus} energy.";
            ShowAnnouncement(message);
        }
    if (player.ability == AbilityType.Herbalist && cardType == "Herb")
        {
            float bonus = 12f;
            player.ChangeEnergy(bonus);
            string message = $"{player.playerName} found herbs! +{bonus} Energy (Herbalist)";
            ShowAnnouncement(message);
        }
}
    public float GetBuildEnergyCostReduction(PlayerStats player)
    {
        if (player.ability == AbilityType.Carpenter)
        {
            return 0.5f;

        }
        return 1f;
    }

    public void CheckStartOfTurnPassive(PlayerStats player)
    {
        if (!player.hasRolledDice) return;
        
        if (Random.value > 0.5f)
        {
            if (player.ability == AbilityType.Musician || player.ability == AbilityType.Driver)
            {
                float bonus = 5f;
                player.ChangeEnergy(bonus);
                string message = $"{player.playerName} activated passive! +{bonus} Energy.";
                ShowAnnouncement(message);
            }
        }
    }
}