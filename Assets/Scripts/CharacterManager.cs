using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDb characterDb;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public SpriteRenderer artworkSprite;
    public TextMeshProUGUI EnergyText;
    public int selectedOption = 0;
    public GameObject[] characterDisplays; 
    private HashSet<int> usedIndices; 

    void Start()
    {
        UpdateCharacter(selectedOption);
    } 
    public void SetUsedCharacters(HashSet<int> used)
    {
        usedIndices = used;
    }

    public void NextOption()
    {
        int nextIndex = selectedOption;
        int originalIndex = selectedOption;
        
        do 
        {
            nextIndex++;
            if (nextIndex >= characterDb.CharacterCount)
            {
                nextIndex = 0; 
            }            
        } while (usedIndices != null && usedIndices.Contains(nextIndex) && nextIndex != originalIndex);
        
        selectedOption = nextIndex;
        UpdateCharacter(selectedOption);
    }

    public void PreviousOption()
    {
        int prevIndex = selectedOption;
        int originalIndex = selectedOption;

        do
        {
            prevIndex--;
            if (prevIndex < 0)
            {
                prevIndex = characterDb.CharacterCount - 1; 
            }
        } while (usedIndices != null && usedIndices.Contains(prevIndex) && prevIndex != originalIndex);

        selectedOption = prevIndex;
        UpdateCharacter(selectedOption);
    }

    public void UpdateCharacter(int selectedOption)
    {        
        Character character = characterDb.GetCharacter(selectedOption);
        for (int i = 0; i < characterDisplays.Length; i++) 
        {        
            if (characterDisplays[i] != null)
            {
                characterDisplays[i].SetActive(false);
            }
        }
        if (selectedOption >= 0 && selectedOption < characterDisplays.Length && characterDisplays[selectedOption] != null)
        {
            characterDisplays[selectedOption].SetActive(true);
        }
        nameText.text = character.characterName;
        descriptionText.text = character.description;
        EnergyText.text = "Energy: " + character.Energy.ToString();
        artworkSprite.sprite = character.characterSprite;
    }
}