using System;
using System.Globalization;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using LeastSquares;

public class BossFightCameraCoordinator : MonoBehaviour, IBossFightTriggers
{
    public CinemachineVirtualCamera CorridorCam, FightCam;
    private MainCharacterController m_Controller;
    private PlayableDirector m_CinematicDirector;
    public PlayableDirector EntranceUIDirector;
    public GameObject EndGameUI;
    public SteamLeaderboard lb;
    private enum State
    {
        Invalid = 0,
        Corridor = 1,
        PreFightCinematic = 2,
        Fight = 3,
        PostFightCinematic = 4,
        Done = 5,
        Count = 6
    }
    private State m_State = State.Corridor;

    private void Awake()
    {
        lb = FindObjectOfType<SteamLeaderboard>();
        m_Controller = GetComponent<MainCharacterController>()!;
        m_CinematicDirector = GameObject.FindWithTag("MainCamera").GetComponent<PlayableDirector>()!;
        //Application.targetFrameRate = 30;
    }

    private async void Start()
    {
        await Task.Delay(500);
        ScoringSystem ss = GameObject.FindWithTag("ScoringSystem").GetComponent<ScoringSystem>();
        ss.UpdateScore();
        m_CinematicDirector.Play();
        await Task.Delay(500);
        CorridorCam.Priority = 10;
        m_CinematicDirector.Pause();
    }

    public void StartBossFight()
    {
        InternalStartBossFight();
    }

    public void EndBossFight()
    {
        m_State = State.PostFightCinematic;
    }
    
    private async void InternalStartBossFight()
    {
        {
            m_State = State.PreFightCinematic;
            using HLockGuard healthLock = m_Controller.GetComponent<Health>().Lock();
            using HLockGuard playerLock = m_Controller.Lock();
            m_CinematicDirector.Resume();
            EntranceUIDirector.Play();
            await Task.Yield();
            GameObject.FindWithTag("FinalBoss").Trigger<IBossTriggers>(nameof(IBossTriggers.StartFight));
            await Task.Yield();
            FightCam.Priority = 15;
            while (m_State == State.PreFightCinematic)
            {
                await Task.Yield();
            }
            m_Controller.Camera = FightCam;
            m_CinematicDirector.Pause();
        }
        
        while (m_State == State.Fight)
        {
            await Task.Yield();
        }

        {
            using HLockGuard healthLock = m_Controller.GetComponent<Health>().Lock();
            using HLockGuard playerLock = m_Controller.Lock();
            m_CinematicDirector.Resume();
            while (m_State == State.PostFightCinematic)
            {
                await Task.Yield();
            }
            m_CinematicDirector.Pause();
            
            // final panel for end of game
            float currentScore = GameObject.FindWithTag("ScoringSystem").GetComponent<ScoringSystem>().currScore;
            //GameManager.score = (int) currentScore;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            lb.SubmitScore((int)currentScore);
#endif
            Debug.Log("Submitted!!!!! " + ((int)currentScore).ToString());
            EndGameUI.SetActive(true);
            foreach (Transform child in EndGameUI.transform)
            {
                if (child.name == "FinalScoreText")
                {
                    string currentString = child.GetComponent<TextMeshProUGUI>().text;
                    currentString = currentString.Replace("...", currentScore.ToString(CultureInfo.InvariantCulture));
                    child.GetComponent<TextMeshProUGUI>().text = currentString;
                } else if (child.name == "ButtonReturn") {
                    EventSystem.current.SetSelectedGameObject(child.gameObject); // for controller
                }
            }
            while (this != null)
            {
                await Task.Yield();
            }
        }
    }

    public void BossEncounterCinematicEnd()
    {
        m_State = State.Fight;
    }
    
    public void BossFightCinematicEnd()
    {
        m_State = State.Done;
    }
}
