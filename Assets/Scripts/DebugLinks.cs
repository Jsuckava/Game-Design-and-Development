using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class DebugLinks : MonoBehaviour
{
    public CharacterSelection targetScript;

    [ContextMenu("Check Links")]
    public void Check()
    {
        if (targetScript == null)
        {
            targetScript = GetComponent<CharacterSelection>();
            if (targetScript == null)
            {
                Debug.LogError("DebugScript: Cannot find CharacterSelection script!");
                return;
            }
        }

        Debug.Log("--- STARTING DEBUG CHECK ---");

        // Check 1: Database
        if (targetScript.characterDatabase == null)
            Debug.LogError("FAIL: characterDatabase is NULL");
        else
            Debug.Log($"PASS: characterDatabase has {targetScript.characterDatabase.Length} elements.");

        // Check 2: Button
        if (targetScript.selectButton == null)
            Debug.LogError("FAIL: selectButton is NULL");
        else
        {
            Debug.Log($"PASS: selectButton is linked to {targetScript.selectButton.name}");
            var tmp = targetScript.selectButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp == null)
                Debug.LogError("FAIL: selectButton has NO TextMeshProUGUI child!");
            else
                Debug.Log($"PASS: selectButton has TextMeshProUGUI: {tmp.text}");
        }

        // Check 3: Status Text
        if (targetScript.statusText == null)
            Debug.LogError("FAIL: statusText is NULL");
        else
            Debug.Log($"PASS: statusText is linked to {targetScript.statusText.name}");

        // Check 4: Video Players
        if (targetScript.finalPlayer1VideoPlayer == null)
             Debug.LogError("FAIL: finalPlayer1VideoPlayer is NULL");
        else
             Debug.Log("PASS: finalPlayer1VideoPlayer is linked.");

        if (targetScript.finalPlayer2VideoPlayer == null)
             Debug.LogError("FAIL: finalPlayer2VideoPlayer is NULL");
        else
             Debug.Log("PASS: finalPlayer2VideoPlayer is linked.");

        // Check 5: Highlights
        if (targetScript.characterHighlights == null)
            Debug.LogError("FAIL: characterHighlights array is NULL");
        else
        {
            Debug.Log($"PASS: characterHighlights array exists with size {targetScript.characterHighlights.Length}");
            for(int i=0; i<targetScript.characterHighlights.Length; i++)
            {
                if (targetScript.characterHighlights[i] == null)
                    Debug.LogError($"FAIL: Element {i} in Highlights is NULL!");
            }
        }
        
        Debug.Log("--- END DEBUG CHECK ---");
    }
}