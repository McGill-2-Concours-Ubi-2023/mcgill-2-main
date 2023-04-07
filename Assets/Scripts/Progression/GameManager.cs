using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
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
    public GameObject coverUpScreen;
    
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

    private void UpdateProgress()
    {
        float remaining = m_NextProgress - m_CurrentProgress;
        m_CurrentProgress += remaining * Time.deltaTime * 3;
        loadingFillBar.fillAmount = m_CurrentProgress;
    }

    IEnumerator OnSceneLoadProgress(int sceneId)
    {
        isLoading = true;
        m_CurrentProgress = 0.0f;
        m_NextProgress = 0.0f;
        loadingFillBar.fillAmount = m_CurrentProgress;
        const float sceneLoadProgressMax = 0.5f;
        ReportProgress(0.0f, sceneLoadProgressMax);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneId);
        while (!loading.isDone)
        {
            UpdateProgress();
            yield return null;
        }
        
        while (isLoading)
        {
            UpdateProgress();
            yield return null;
        }
        
        loadingScreen.gameObject.SetActive(false);
    }

    private async void Update()
    {
        if (isLoading)
        {
            return;
        }
        
        GameObject player = GameObject.FindWithTag("Player");
        if (!player)
        {
            return;
        }

        float3 pos = player.transform.position;
        
        if (pos.y < -1)
        {
            coverUpScreen.gameObject.SetActive(true);
            await Task.Yield();
            pos.y = 0.5f;
            float3 roomPos = DungeonRoom.GetActiveRoom().transform.position;
            pos.xz = roomPos.xz;
            player.transform.position = pos;
            await Task.Delay(500);
            coverUpScreen.gameObject.SetActive(false);
        }
    }
}
