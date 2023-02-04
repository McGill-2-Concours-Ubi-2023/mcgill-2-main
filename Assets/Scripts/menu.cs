using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
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

    public void SceneLoad(int i)
    {
        SceneManager.LoadScene(i);
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
