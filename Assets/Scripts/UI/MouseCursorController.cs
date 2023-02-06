using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MouseCursorController : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorHighlight;

    private void OnEnable()
    {
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void ChangeCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }   
}
