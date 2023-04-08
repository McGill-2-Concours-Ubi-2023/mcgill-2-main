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
        m_CinematicDirector.Pause();
    }

    public void StartBossFight()
    {
        InternalStartBossFight();
    }
    
    private async void InternalStartBossFight()
    {
        using HLockGuard healthLock = m_Controller.GetComponent<Health>().Lock();
        using HLockGuard playerLock = m_Controller.Lock();
        m_CinematicDirector.Resume();
        await Task.Yield();
        GameObject.FindWithTag("FinalBoss").Trigger<IBossTriggers>(nameof(IBossTriggers.StartAttack));
        await Task.Yield();
        FightCam.Priority = 15;
        m_Controller.Camera = FightCam;
        while (m_CinematicDirector.state == PlayState.Playing)
        {
            await Task.Yield();
        }
    }
}
