using UnityEngine;

public class BuildTileController : MonoBehaviour
{
    public float baseEnergyCost = 15f; 
    private const float CARPENTER_MULTIPLIER = 0.5f;

    private BahayKuboTracker bahayKuboTracker;
    private PopupController popupController;

    void Start()
    {
        bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        popupController = FindFirstObjectByType<PopupController>();
    }

    public void StartBuilding(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
            return;
        }

        float finalEnergyCost = baseEnergyCost;
        if (playerStats.CheckAbility() == AbilityType.Carpenter)
        {
            finalEnergyCost *= CARPENTER_MULTIPLIER;
        }
        
        if (playerStats.currentEnergy < finalEnergyCost)
        {
            popupController.ShowSimplePopup("Not Enough Energy!", "You need " + finalEnergyCost + " energy to build.");
            return;
        }

        string partBuilt = bahayKuboTracker.AttemptBuild(playerStats);

        if (partBuilt == "Complete")
        {
            popupController.ShowSimplePopup("All Done!", "The BahayKubo is finished!");
        }
        else if (partBuilt == "NotEnoughBamboo")
        {
            popupController.ShowSimplePopup("Not Enough Materials!", $"You need {bahayKuboTracker.haligi_bambooCost} Bamboo to build the Haligi.");
        }
        else if (partBuilt == "NotEnoughHardwood")
        {
            popupController.ShowSimplePopup("Not Enough Materials!", $"You need {bahayKuboTracker.sahig_hardwoodCost} Hardwood to build the Sahig.");
        }
        else if (partBuilt == "NotEnoughSawali")
        {
            popupController.ShowSimplePopup("Not Enough Materials!", $"You need {bahayKuboTracker.pader_sawaliCost} Sawali to build the Pader.");
        }
        else if (partBuilt == "NotEnoughNipa")
        {
            popupController.ShowSimplePopup("Not Enough Materials!", $"You need {bahayKuboTracker.bubong_nipaCost} Nipa Leaves to build the Bubong.");
        }
        else
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            popupController.ShowSimplePopup("Part Built!", "You used " + finalEnergyCost + " energy to build the " + partBuilt + "!");
        }
    }
}