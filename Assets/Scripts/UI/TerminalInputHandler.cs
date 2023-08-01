using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TerminalInputHandler : MonoBehaviour
{
    private bool m_IsTerminalActive = false;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnToggleConsole()
    {
        //m_IsTerminalActive = true;
        //transform.GetChild(0).gameObject.SetActive(m_IsTerminalActive);
    }

    /*private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Period))
        {
            OnToggleConsole();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Return))
        {
            Task t = GetComponentInChildren<TerminalManager>()?.TextSubmit();
            if (t != null)
            {
                await t;
            }
        }
    }*/
}
