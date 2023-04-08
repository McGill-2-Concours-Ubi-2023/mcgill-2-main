using System;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class BossFightCameraCoordinator : MonoBehaviour, IBossFightTriggers
{
    public CinemachineVirtualCamera CorridorCam, FightCam;
    private MainCharacterController m_Controller;

    private void Awake()
    {
        m_Controller = GetComponent<MainCharacterController>();
    }

    public void StartBossFight()
    {
        InternalStartBossFight();
    }
    
    private async void InternalStartBossFight()
    {

    }
}
