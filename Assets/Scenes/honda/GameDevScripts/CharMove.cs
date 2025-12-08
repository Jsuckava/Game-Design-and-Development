using UnityEngine;

public class CharMove : MonoBehaviour
{
    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.gameObject.CompareTag("Character"))
        {
            rb = GetComponent<Rigidbody2D>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.CompareTag("Character"))
        {
            rb.linearVelocity = new Vector2(400, rb.linearVelocity.y);
        }
    }
    
    public void OnAttack()
    {
        Tower.towerHealth -= 20;
        Debug.Log("Tower Health: " + Tower.towerHealth);
    }
}
