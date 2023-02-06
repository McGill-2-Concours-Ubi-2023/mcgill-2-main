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
    private RectTransform cursorTransform; //cursor = 2D image
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
    private  int mouseSize;

    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private void OnEnable()
    {
        scaleFactor = Screen.currentResolution.width / Screen.currentResolution.height;
        mouseSize = 100 * scaleFactor; 
        //ChangeCursor(defaultCursorTexture);
        Cursor.lockState = CursorLockMode.Confined;
        cursorTransform.gameObject.SetActive(false);
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

        if(cursorTransform != null)
        {
            //initialize the cursor's position with the anchored position of the cursor Image
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        GetComponent<PlayerInput>().onControlsChanged += OnControlsChanged;        
    }

    private void Start()
    {
        ChangeCursor(defaultCursorTexture);
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateVirtualCursor;
        GetComponent<PlayerInput>().onControlsChanged -= OnControlsChanged;
    }

    public void ChangeCursor(Texture2D cursor)
    {
        var newWidth = Screen.currentResolution.width / mouseSize;
        var newHeight = Screen.currentResolution.height / mouseSize;
        /*Texture2D newTexture = new Texture2D(newWidth, newHeight);
        Graphics.Blit(originalTexture, newTexture, ImageEffects.bicubicDownsamplerMaterial);*/
        Texture2D scaledCursor =  Utils.ScaleTexture(cursor, Screen.currentResolution.width / mouseSize,
            Screen.currentResolution.height / mouseSize);
        Cursor.SetCursor(scaledCursor, new Vector2(scaledCursor.width/2, scaledCursor.height/2), CursorMode.Auto);
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
        AnchorCursor(newMousePosition);

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

    private void AnchorCursor(Vector2 position)
    {
        //set the new anchoredPosition
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(virtualCanvas.GetComponent<RectTransform>(), position,
            virtualCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if(input.currentControlScheme == mouseScheme && previousControlsScheme != mouseScheme)
        {
            Cursor.visible = true;
            cursorTransform.gameObject.SetActive(false);
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlsScheme = mouseScheme;
        } else if (input.currentControlScheme == gamepadScheme && previousControlsScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previousControlsScheme = gamepadScheme;
            Cursor.visible = false;
        }
    }

    private bool IsMouseOutsideViewPort()
    {
        var mouseViewPosition = Camera.main.ScreenToViewportPoint(currentMouse.position.ReadValue());
        var isOutside = mouseViewPosition.x < 0 || mouseViewPosition.x > 1 || mouseViewPosition.y < 0 || mouseViewPosition.y > 1;
        return isOutside;
    }

    public void PrintMessage()//Button OnClick event
    {
        if(Gamepad.current != null)
        {
            if (!Gamepad.current.buttonSouth.IsPressed() || !currentMouse.leftButton.IsPressed())
            {
                Debug.Log("BUTTON CLICKED!");
            }
        }      
    }

    public void OnHoverFeedback()
    {
        cursorSensitivity /= cursorFeedBackDampening;
    }

    public void OnExitFeedback()
    {
        cursorSensitivity *= cursorFeedBackDampening;
    }
}
