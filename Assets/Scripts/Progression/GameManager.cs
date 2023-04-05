using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject loadingScreen;
    public Image loadingFillBar;
    public Canvas LoadingCanvas;
    private static bool m_IsLoading = false;
    private float m_NextProgress = 0.0f;
    private float m_CurrentProgress = 0.0f;
    
    public static bool isLoading
    {
        get => m_IsLoading;
        set
        {
            if (value == m_IsLoading)
            {
                throw new Exception("Illegal state change");
            }
            m_IsLoading = value;
        }
    }
    
    public void ReportProgress(float progress, float nextProgress)
    {
        //m_CurrentProgress = progress;
        //loadingFillBar.fillAmount = progress;
        m_NextProgress = nextProgress;
    }

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(LoadingCanvas);
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public void LoadScene(int sceneId)
    {
        loadingScreen.gameObject.SetActive(true);
        StartCoroutine(OnSceneLoadProgress(sceneId));
    }

    IEnumerator OnSceneLoadProgress(int sceneId)
    {
        isLoading = true;
        m_CurrentProgress = 0.0f;
        m_NextProgress = 0.0f;
        loadingFillBar.fillAmount = m_CurrentProgress;
        ReportProgress(0.0f, 0.0f);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneId);
        while (!loading.isDone)
        {
            yield return null;
        }
        
        while (isLoading)
        {
            float remaining = m_NextProgress - m_CurrentProgress;
            m_CurrentProgress += remaining * Time.deltaTime * 3;
            loadingFillBar.fillAmount = m_CurrentProgress;
            yield return null;
        }
        
        loadingScreen.gameObject.SetActive(false);
    }
}
