using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        transform.Translate(move * speed * Time.deltaTime);
    }
}
