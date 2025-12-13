using UnityEngine;

public class BuildTileController : MonoBehaviour
{
    [Header("House Parts")]
    public GameObject haligiObject;
    public GameObject sahigObject;
    public GameObject paderObject;
    public GameObject bubongObject;

    [Header("Settings")]
    public float baseEnergyCost = 15f; 
    private const float CARPENTER_MULTIPLIER = 0.5f;

    private BahayKuboTracker bahayKuboTracker;
    private PopupController popupController;

    void Start()
    {
        bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        popupController = FindFirstObjectByType<PopupController>();

        UpdateVisuals();
    }

    void Update()
    {
        if (Application.isEditor && !Timer.IsGameOver) 
        {
            UpdateVisuals();
        }
    }

    public void StartBuilding(GameObject player)
    {
        if (Timer.IsGameOver) return;

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        float finalEnergyCost = baseEnergyCost;
        if (playerStats.CheckAbility() == AbilityType.Carpenter)
        {
            finalEnergyCost *= CARPENTER_MULTIPLIER;
        }
        
        if (playerStats.currentEnergy < finalEnergyCost)
        {
            if (popupController != null)
                popupController.ShowSimplePopup("Not Enough Energy!", "You need " + finalEnergyCost + " energy to build.");
            return;
        }

        string partBuilt = bahayKuboTracker.AttemptBuild(playerStats);

        if (partBuilt == "Complete")
        {
            UpdateVisuals();
            if (popupController != null)
                popupController.ShowSimplePopup("All Done!", "The BahayKubo is finished!");
        }
        else if (partBuilt == "Haligi")
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            ShowSuccessPopup(partBuilt, finalEnergyCost);
            UpdateVisuals();
        }
        else if (partBuilt == "Sahig")
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            ShowSuccessPopup(partBuilt, finalEnergyCost);
            UpdateVisuals();
        }
        else if (partBuilt == "Pader")
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            ShowSuccessPopup(partBuilt, finalEnergyCost);
            UpdateVisuals();
        }
        else if (partBuilt == "Bubong")
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            ShowSuccessPopup(partBuilt, finalEnergyCost);
            UpdateVisuals();
        }
        else if (partBuilt == "NotEnoughBamboo")
        {
            ShowErrorPopup($"You need {bahayKuboTracker.haligi_bambooCost} Bamboo to build the Haligi.");
        }
        else if (partBuilt == "NotEnoughHardwood")
        {
            ShowErrorPopup($"You need {bahayKuboTracker.sahig_hardwoodCost} Hardwood to build the Sahig.");
        }
        else if (partBuilt == "NotEnoughSawali")
        {
            ShowErrorPopup($"You need {bahayKuboTracker.pader_sawaliCost} Sawali to build the Pader.");
        }
        else if (partBuilt == "NotEnoughBambooForPader")
        {
            ShowErrorPopup($"You have Sawali, but need more Bamboo ({bahayKuboTracker.pader_bambooCost} total) for the Pader.");
        }
        else if (partBuilt == "NotEnoughNipa")
        {
            ShowErrorPopup($"You need {bahayKuboTracker.bubong_nipaCost} Nipa Leaves to build the Bubong.");
        }
        else if (partBuilt == "NotEnoughBambooForBubong")
        {
            ShowErrorPopup($"You have Nipa, but need more Bamboo ({bahayKuboTracker.bubong_bambooCost} total) for the Bubong.");
        }
    }

    private void ShowSuccessPopup(string part, float energy)
    {
        if (Timer.IsGameOver) return;

        if (popupController != null)
            popupController.ShowSimplePopup("Part Built!", $"You used {energy} energy to build the {part}!");
    }

    private void ShowErrorPopup(string message)
    {
        if (Timer.IsGameOver) return;

        if (popupController != null)
            popupController.ShowSimplePopup("Not Enough Materials!", message);
    }

    public void UpdateVisuals()
    {
        // if (Timer.IsGameOver) return;

        if (bahayKuboTracker == null) return;

        if (haligiObject != null) haligiObject.SetActive(bahayKuboTracker.haligiBuilt);
        if (sahigObject != null) sahigObject.SetActive(bahayKuboTracker.sahigBuilt);
        if (paderObject != null) paderObject.SetActive(bahayKuboTracker.paderBuilt);
        if (bubongObject != null) bubongObject.SetActive(bahayKuboTracker.bubongBuilt);
    }
}