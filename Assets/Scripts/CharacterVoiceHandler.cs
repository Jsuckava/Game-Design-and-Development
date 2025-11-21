using UnityEngine;

public class CharacterVoiceHandler : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource audioPlayer;     
    public AudioClip[] characterVoices; 
    public void PlayCharacterVoice(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterVoices.Length)
        {
            audioPlayer.Stop(); 
            audioPlayer.PlayOneShot(characterVoices[characterIndex]);
        }
    }
}