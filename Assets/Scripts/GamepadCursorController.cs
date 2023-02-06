using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class GamepadCursorController : MonoBehaviour
{
    private Mouse virtualMouse;
    [SerializeField]
    private RectTransform cursorTransform; //cursor = 2D image
    [SerializeField][Range(10,2000)]
    private float cursorSensitivity = 1500;
    private float cursorFeedBackDampening = 2;
    [SerializeField]
    private Canvas virtualCanvas;

    private void OnEnable()
    {
        if(virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        } else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }
        //Link the virtual mouse to the playerInput component
        InputUser.PerformPairingWithDevice(virtualMouse, GetComponent<PlayerInput>().user);
        //Subscribe to the delegate by adding "UpdateVirtualCursor" to the invocation list
        InputSystem.onAfterUpdate += UpdateVirtualCursor;

        if(cursorTransform != null)
        {
            //initialize the cursor's position with the anchored position of the cursor Image
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateVirtualCursor;
    }

    private void UpdateVirtualCursor()
    {
        if(virtualMouse == null || Gamepad.current == null)
        {
            return; //do nothing
        }

        Vector2 joyStickInput = Gamepad.current.leftStick.ReadValue(); //returns a Vector2 - input value
        var deltaValue = joyStickInput * cursorSensitivity * Time.deltaTime; //calculate the mouse offset to be added later

        Vector2 currentMousePosition = virtualMouse.position.ReadValue(); //read current mouse position
        Vector2 newMousePosition = currentMousePosition + deltaValue; //recalculate the virtual mouse position

        //Bound the virtual mouse position with the dimensions of the screen
        newMousePosition.x = Mathf.Clamp(newMousePosition.x, 0, Screen.width);
        newMousePosition.y = Mathf.Clamp(newMousePosition.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newMousePosition);
        //Mouse.delta represents the change in mousePosition since last frame
        InputState.Change(virtualMouse.delta, deltaValue);
        AnchorCursor(newMousePosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        //set the new anchoredPosition
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(virtualCanvas.GetComponent<RectTransform>(), position,
            virtualCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }


    public void OnClick()
    {
        virtualMouse.CopyState<MouseState>(out var mouseState);
        mouseState.WithButton(MouseButton.Left, Gamepad.current.buttonSouth.IsPressed());
        InputState.Change(virtualMouse, mouseState);
    }

    public void OnButtonEnter()
    {
        cursorSensitivity /= cursorFeedBackDampening;
    }

    public void OnButtonExit()
    {
        cursorSensitivity *= cursorFeedBackDampening;
    }


}
