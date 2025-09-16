using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectMovement2 : MonoBehaviour
{
    public float speed2 = 5f;

    void Update()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed) move.y += 1;
        if (Keyboard.current.downArrowKey.isPressed) move.y -= 1;
        if (Keyboard.current.leftArrowKey.isPressed) move.x -= 1;
        if (Keyboard.current.rightArrowKey.isPressed) move.x += 1;

        if (move != Vector2.zero)
        {
            move = move.normalized;
            transform.Translate(move * speed2 * Time.deltaTime);
        }
    }
}
