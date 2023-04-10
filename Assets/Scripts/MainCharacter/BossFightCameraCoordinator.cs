using System;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightCameraCoordinator : MonoBehaviour, IBossFightTriggers
{
    public CinemachineVirtualCamera CorridorCam, FightCam;
    private MainCharacterController m_Controller;
    private PlayableDirector m_CinematicDirector;
    public PlayableDirector EntranceUIDirector;
    private enum State
    {
        Corridor,
        PreFightCinematic,
        Fight,
        PostFightCinematic,
        Done
    }
    private State m_State = State.Corridor;

    private void Awake()
    {
        m_Controller = GetComponent<MainCharacterController>()!;
        m_CinematicDirector = GameObject.FindWithTag("MainCamera").GetComponent<PlayableDirector>()!;
    }

    private async void Start()
    {
        await Task.Delay(500);
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
            m_Controller.Camera = FightCam;
            while (m_State == State.PreFightCinematic)
            {
                await Task.Yield();
            }
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
