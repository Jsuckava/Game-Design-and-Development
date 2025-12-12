using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(clickSound);
    }
}