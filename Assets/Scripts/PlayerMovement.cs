using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class AttackTeleport : MonoBehaviour
{
    [Header("Distance Settings")]
    public float distanceFromEnemy = 2.5f;

    [Header("Timing Settings")]
    public bool useAutomaticVideoLength = false; // Uncheck this to use manual time
    public float manualDuration = 3.0f; // Type your video length here!

    private Vector3 originalPosition;
    private VideoPlayer myVideoPlayer;

    void Start()
    {
        originalPosition = transform.position;
        myVideoPlayer = GetComponent<VideoPlayer>();
    }

    public void TeleportAndAttack(Transform enemyTarget)
    {
        StartCoroutine(AttackRoutine(enemyTarget));
    }

    private IEnumerator AttackRoutine(Transform enemy)
    {
        Vector3 direction = (transform.position - enemy.position).normalized;
        Vector3 attackSpot = enemy.position + (direction * distanceFromEnemy);
        transform.position = attackSpot;

        if(myVideoPlayer != null) 
        {
            myVideoPlayer.Stop(); 
            myVideoPlayer.Play();
        }

        float waitTime = manualDuration;

        if (useAutomaticVideoLength && myVideoPlayer != null && myVideoPlayer.clip != null)
        {
            waitTime = (float)myVideoPlayer.clip.length;
        }

        yield return new WaitForSeconds(waitTime);

        transform.position = originalPosition;
    }
}