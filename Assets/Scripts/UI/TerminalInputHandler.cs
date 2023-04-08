using System.Collections;
using System.Collections.Generic;
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
        m_IsTerminalActive = true;
        transform.GetChild(0).gameObject.SetActive(m_IsTerminalActive);
    }
}
