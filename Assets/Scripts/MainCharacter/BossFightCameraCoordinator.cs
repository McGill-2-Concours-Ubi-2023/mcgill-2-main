using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class BossFightCameraCoordinator : MonoBehaviour, IBossFightTriggers
{
    public CinemachineVirtualCamera CorridorCam, CutSceneCam, FightCam;
    
    public void StartBossFight()
    {
        InternalStartBossFight();
    }
    
    private async void InternalStartBossFight()
    {
        CutSceneCam.gameObject.SetActive(true);
        await Task.Delay(5000);
        CorridorCam.gameObject.SetActive(false);
    }
}
