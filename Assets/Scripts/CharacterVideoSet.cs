using UnityEngine;
using UnityEngine.Video; // Use the Video namespace for MP4 assets

[System.Serializable] 
public class CharacterVideoSet
{
    public string characterName;
    
    public VideoClip idleClip;
    public VideoClip hitClip;    
    public VideoClip attack1Clip; 
    public VideoClip kickClip;    
    public VideoClip superClip;   
}