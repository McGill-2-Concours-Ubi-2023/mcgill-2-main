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
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneId);
        while (!loading.isDone)
        {
            float progressValue = Mathf.Clamp01(loading.progress/0.9f);
            loadingFillBar.fillAmount = progressValue;
            yield return null;
        }
        
        while (isLoading)
        {
            yield return null;
        }
        
        loadingScreen.gameObject.SetActive(false);
    }
}
