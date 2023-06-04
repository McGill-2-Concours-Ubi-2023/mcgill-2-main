using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
public class Menu : MonoBehaviour
{
    public bool cursor;
    [SerializeField]
    private GameObject activeWindow;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GameObject selectionButton;

    void Start()
    {
        if (!eventSystem) eventSystem = GetComponent<EventSystem>();
        if (!cursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
            Cursor.visible = true;
    }

    public void Shut()
    {
        Application.Quit();
    }

    public void SetActiveWindow(GameObject window)
    {
        activeWindow = window;
    }

    public void SetSelectionButton(GameObject button)
    {
        selectionButton = button;
    }

    public void OnCancel() //press right button on gamepad
    {
        Debug.Log("escaping active window");
        if (activeWindow)
        {
            activeWindow.SetActive(false);
            eventSystem.SetSelectedGameObject(selectionButton);
        }
    }

    public void URL(string url)
    {
        Application.OpenURL(url);
    }
    public void LoadScene(int i) {
        GameManager.Instance.LoadScene(i);
    }
    public void LoadScene(string s)
    {
        throw new System.NotImplementedException();
    }
}
