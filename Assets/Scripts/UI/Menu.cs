using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
public class Menu : MonoBehaviour
{
    public bool cursor;
    
    void Start()
    {
       /* if (!cursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
            Cursor.visible = true;*/
    }
    
    public void Shut()
    {
        Application.Quit();
    }
    
    public void URL(string url)
    {
        Application.OpenURL(url);
    }
}
