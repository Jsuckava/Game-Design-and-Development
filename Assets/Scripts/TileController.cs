using UnityEngine;

public class BuildTileController : MonoBehaviour
{
    public float baseEnergyCost = 15f; 
    private const float CARPENTER_MULTIPLIER = 0.5f;

    public void StartBuilding(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found on the player!");
            return;
        }

        float finalEnergyCost = baseEnergyCost;
        
        if (playerStats.GetActiveAbility() == AbilityType.Carpenter)
        {
            finalEnergyCost *= CARPENTER_MULTIPLIER;
            Debug.Log("Carpenter Ability: Energy cost reduced!");
            
            playerStats.ConsumeAbility(); 
        }
        
        if (playerStats.currentEnergy >= finalEnergyCost)
        {
            playerStats.ChangeEnergy(-finalEnergyCost);
            Debug.Log(player.name + " successfully built a part of the Bahay Kubo.");
        }
        else
        {
            Debug.Log(player.name + " does not have enough energy to build!");
        }
    }
}