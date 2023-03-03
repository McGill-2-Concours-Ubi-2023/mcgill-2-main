using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class CursorController : MonoBehaviour
{
    private Mouse virtualMouse;
    private Mouse currentMouse;
    [SerializeField]
    private RectTransform virtualCursorTransform, realCursorTransform; //cursor = 2D image
    [SerializeField][Range(10,2000)]
    private float cursorSensitivity = 1500;
    private float cursorFeedBackDampening = 2;
    [SerializeField]
    private Canvas virtualCanvas;
    [SerializeField]
    private float cursorPadding = 20f; //adjust depending on cursor Texture size
    private string previousControlsScheme = "";
    [SerializeField]
    private Texture2D defaultCursorTexture;
    [SerializeField]
    public Texture2D highlightCursorTexture;
    private int scaleFactor;

    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private void Update()
    {
        if (IsMouseOutsideViewPort())
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

    }

    private void OnEnable()
    {
        scaleFactor = Screen.currentResolution.width / Screen.currentResolution.height;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        virtualCursorTransform.gameObject.SetActive(false);
        realCursorTransform.gameObject.SetActive(true);
        currentMouse = Mouse.current;

        if (virtualMouse == null)
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
        InputSystem.onAfterUpdate += UpdateRealCursor;

        if(virtualCursorTransform != null)
        {
            //initialize the cursor's position with the anchored position of the cursor Image
            Vector2 position = virtualCursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        GetComponent<PlayerInput>().onControlsChanged += OnControlsChanged;        
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateVirtualCursor;
        InputSystem.onAfterUpdate -= UpdateRealCursor;
        GetComponent<PlayerInput>().onControlsChanged -= OnControlsChanged;
    }

    private void UpdateRealCursor()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        AnchorCursor(mousePosition, realCursorTransform);
    }

    private void UpdateVirtualCursor()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return; //do nothing
        }

        Vector2 joyStickInput = Gamepad.current.leftStick.ReadValue(); //returns a Vector2 - input value
        var deltaValue = joyStickInput * cursorSensitivity * Time.deltaTime; //calculate the mouse offset to be added later

        Vector2 currentMousePosition = virtualMouse.position.ReadValue(); //read current mouse position
        Vector2 newMousePosition = currentMousePosition + deltaValue; //recalculate the virtual mouse position

        //Bound the virtual mouse position with the dimensions of the screen
        newMousePosition.x = Mathf.Clamp(newMousePosition.x, cursorPadding, Screen.width - cursorPadding);
        newMousePosition.y = Mathf.Clamp(newMousePosition.y, cursorPadding, Screen.height - cursorPadding);

        InputState.Change(virtualMouse.position, newMousePosition);
        //Mouse.delta represents the change in mousePosition since last frame
        InputState.Change(virtualMouse.delta, deltaValue);
        AnchorCursor(newMousePosition, virtualCursorTransform);

        OnGamepadClick();

        OnGamePadRB();           
    }

    private void OnGamepadClick()
    {
        virtualMouse.CopyState<MouseState>(out var mouseState);
        mouseState.WithButton(MouseButton.Left, Gamepad.current.buttonSouth.IsPressed());
        InputState.Change(virtualMouse, mouseState);
        // TODO: Add vibrations 
    }

    private void OnGamePadRB()
    {
        //TODO: Change tab
    }

    private void AnchorCursor(Vector2 position, RectTransform transform)
    {
        //set the new anchoredPosition
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(virtualCanvas.GetComponent<RectTransform>(), position,
            virtualCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPosition);
        transform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if(input.currentControlScheme == mouseScheme && previousControlsScheme != mouseScheme)
        {
            realCursorTransform.gameObject.SetActive(true);
            virtualCursorTransform.gameObject.SetActive(false);
            AnchorCursor(virtualMouse.position.ReadValue(), realCursorTransform);
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlsScheme = mouseScheme;
        } else if (input.currentControlScheme == gamepadScheme && previousControlsScheme != gamepadScheme)
        {
            realCursorTransform.gameObject.SetActive(false);
            virtualCursorTransform.gameObject.SetActive(true);
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue(), virtualCursorTransform);
            previousControlsScheme = gamepadScheme;
        }
    }

    private bool IsMouseOutsideViewPort()
    {
        var mouseViewPosition = Camera.main.ScreenToViewportPoint(currentMouse.position.ReadValue());
        var isOutside = mouseViewPosition.x < 0 || mouseViewPosition.x > 1 || mouseViewPosition.y < 0 || mouseViewPosition.y > 1;
        return isOutside;
    }

    public void OnDPadNextButton()
    {
        var dpadValue = Gamepad.current.dpad.ReadValue();
        if (dpadValue.x == 1)
        {
            //TODO: Select button right
        }
        else if (dpadValue.x == -1)
        {
            //TODO: Select button left
        }
        if(dpadValue.y == 1)
        {
            //TODO: Select button up
        } else if(dpadValue.y == -1) 
        {
            //TODO: Select button down    
        }
    }

    public void PrintMessage()//Button OnClick event
    {
        if (IsClickSourceUnique())
        {
            Debug.Log("BUTTON CLICKED!");
        }
    }

    public void OnGamepadClickRumble()
    {
        if (IsClickSourceUnique())
        {
            FindObjectOfType<Rumbler>().TriggerVibration(0.1f, 0.3f, 0.3f);
        }     
    }

    public bool IsClickSourceUnique()
    {
        bool onGamepadRelease = true;
        if(Gamepad.current != null)
        {
            onGamepadRelease = !Gamepad.current.buttonSouth.IsPressed();
        }
        return !currentMouse.leftButton.IsPressed() && onGamepadRelease;
    }

    public void OnHoverFeedback() //Must be called in inspector via OnClick button action
    {
        cursorSensitivity /= cursorFeedBackDampening;
    }

    public void OnExitFeedback() //Must be called in inspector via OnClick button action
    {
        cursorSensitivity *= cursorFeedBackDampening;
    }
}
