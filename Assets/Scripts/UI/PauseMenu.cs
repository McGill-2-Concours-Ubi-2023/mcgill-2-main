using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private State m_State = State.Playing;

    private enum State
    {
        Invalid = 0,
        Playing = 1,
        Paused = 2,
        Count = 3
    }

    private void Start()
    {
        m_InputActionAsset = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions;
        m_InputActionAsset["Pause"].performed += ctx => PausePressed();
    }

    private void PausePressed() {
        if (pauseMenuPanel != null)
        {
            if (!pauseMenuPanel.activeSelf)
            {
                m_State = State.Paused;
                PauseRoutine();
                PauseTime();
            } else {
                Resume();
            }
        }
        else
        {
            Resume();
        }
    }
    
    private async void PauseRoutine()
    {
        using HLockGuard playerLock = GameObject.FindWithTag("Player").GetComponent<MainCharacterController>().Lock();
        await Task.Yield();
        while (m_State == State.Paused)
        {
            await Task.Yield();
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
        m_State = State.Playing;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Exit() {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void MainMenu() {
        PlayerPrefs.DeleteAll();
        m_State = State.Playing;
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
