using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineCameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;
    private CinemachineBasicMultiChannelPerlin perlinChannel;
    public NoiseSettings[] noiseSettings;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlinChannel = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinChannel.m_FrequencyGain = 0;
        perlinChannel.m_AmplitudeGain = 0;
    }

    public void StandardCameraShake(float intensity, float frequencyGain, int noiseSettingsId)
    {
        SetNoiseSettings(noiseSettingsId);
        perlinChannel.m_AmplitudeGain = intensity;
        perlinChannel.m_FrequencyGain = frequencyGain;;
    }

    public void StopCameraShake()
    {
        perlinChannel.m_AmplitudeGain = 0;
    }

    public void SetNoiseSettings(int noiseSettingsId)
    {
        perlinChannel.m_NoiseProfile = noiseSettings[noiseSettingsId];
    }

    public void WobbleGravityShake(float intensity, float frequencyGain, int noiseSettingsId)
    {
        SetNoiseSettings(noiseSettingsId);
        perlinChannel.m_AmplitudeGain = intensity;
        perlinChannel.m_FrequencyGain = frequencyGain;
    }

    public void OnClickShake()
    {
        if(FindObjectOfType<CursorController>().IsClickSourceUnique())
        {
            Debug.Log("SHAKING!");
            StandardCameraShake(4.0f, 1, 0);
        }       
    }
}
