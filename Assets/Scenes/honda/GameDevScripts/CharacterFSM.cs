using System;
using UnityEngine;

public class CharacterFSM : MonoBehaviour
{
    public CharState currentState;
    public Animator attackAnim;
    public CharMove characterMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = CharState.Walking;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Tower"))
        {
            Debug.Log("Attacking Tower");
            attackAnim.SetBool("isAttacking", true);
            currentState = CharState.Attacking;
            characterMovement.enabled = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            Debug.Log("Stopped Attacking Tower");
            attackAnim.SetBool("isAttacking", false);
            currentState = CharState.Walking;
            characterMovement.enabled = true;
        }
    }
}

public enum CharState
{
    Idle,
    Walking,
    Attacking
}