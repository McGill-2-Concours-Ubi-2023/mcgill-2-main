using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenuPanel;
    private InputActionAsset m_InputActionAsset;
    public GameObject button1, button2, button3; 
    private GameObject selectedButton;  

    private void Start()
    {
        m_InputActionAsset = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions;
        m_InputActionAsset["Pause"].performed += ctx => PausePressed();
    }

    private void PausePressed() {
        if (pauseMenuPanel != null)
        {
            if(!pauseMenuPanel.activeSelf)
            PauseTime();
        }
        else
        {
            Resume();
        }
    }

    public void PauseTime() {
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        selectedButton = button1;
        EventSystem.current.SetSelectedGameObject(button1);
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume() {
        Time.timeScale = 1f;
        if(pauseMenuPanel != null)
        pauseMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Exit() {
        Application.Quit();
    }

    public void MainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ) {
            PausePressed();
        }
    }
}
