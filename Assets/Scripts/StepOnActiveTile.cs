using UnityEngine;

public class StepOnActiveTile : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player stepped on the tile!");
        }
        }
    }
