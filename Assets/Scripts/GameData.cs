using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "FightingGame/GameData")]
public class GameData : ScriptableObject
{
    public int player1CharacterID = -1;
    public int player2CharacterID = -1; 

    public void ResetSelections()
    {
        player1CharacterID = -1;
        player2CharacterID = -1;
    }
}