using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonHelper : MonoBehaviour
{
    private TurnManager turnManager;
    private PopupController popupController;
    private Button myButton;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        popupController = FindFirstObjectByType<PopupController>();
        myButton = GetComponent<Button>();

        if (myButton != null)
        {
            myButton.onClick.AddListener(OnInventoryClicked);
        }
    }

    private void OnInventoryClicked()
    {
        if (turnManager == null || popupController == null) return;

        PlayerStats currentPlayer = turnManager.GetCurrentActivePlayer();

        if (currentPlayer != null)
        {
            popupController.ShowInventory(currentPlayer);
        }
    }
}
