using UnityEngine;
using UnityEngine.Video;
using System;

public class CharacterVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private VideoClip idleClip;
    private VideoClip attack1Clip;
    private VideoClip hitClip;
    // Removed: private VideoClip defeatClip;
    
    private VideoClip kickClip;
    private VideoClip superClip;

    public event Action OnVideoFinished;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += CheckVideoFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer component not found on character!");
        }
    }

    public void SetupCharacter(CharacterVideoSet data)
    {
        idleClip = data.idleClip;
        attack1Clip = data.attack1Clip;
        hitClip = data.hitClip;
        // Removed: defeatClip = data.defeatClip;
        
        kickClip = data.kickClip;
        superClip = data.superClip;

        PlayIdle();
    }

    private void CheckVideoFinished(VideoPlayer vp)
    {
        if (vp.isLooping == false)
        {
            OnVideoFinished?.Invoke();
        }
    }

    public void PlayIdle()
    {
        if (videoPlayer == null || idleClip == null) return;
        videoPlayer.clip = idleClip;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

    public void PlayAttack1()
    {
        if (videoPlayer == null || attack1Clip == null) return;
        videoPlayer.clip = attack1Clip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    public void PlayHit()
    {
        if (videoPlayer == null || hitClip == null) return;
        videoPlayer.clip = hitClip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    public void PlayKick()
    {
        if (videoPlayer == null || kickClip == null) return;
        videoPlayer.clip = kickClip; 
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    public void PlaySuper()
    {
        if (videoPlayer == null || superClip == null) return;
        videoPlayer.clip = superClip; 
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }
}