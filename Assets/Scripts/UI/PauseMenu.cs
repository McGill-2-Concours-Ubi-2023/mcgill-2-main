using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenuPanel; 
    public void PauseTime() {
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume() {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Exit() {
        Application.Quit();
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ) {
            if (!pauseMenuPanel.activeSelf)
            {
                PauseTime();
            }
            else {
                Resume();
            }
                
        }
    }
}
