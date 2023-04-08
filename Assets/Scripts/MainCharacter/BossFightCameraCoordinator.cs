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
        m_CinematicDirector.Resume();
    }
}
