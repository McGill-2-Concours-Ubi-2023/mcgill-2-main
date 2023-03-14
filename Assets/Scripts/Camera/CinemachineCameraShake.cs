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

    public void SantardCameraShake(float intensity, float timer, float frequencyGain, int noiseSettingsId)
    {
        SetNoiseSettings(noiseSettingsId);
        perlinChannel.m_AmplitudeGain = intensity;
        perlinChannel.m_FrequencyGain = frequencyGain;
        shakeTimer = timer;
        Shake();
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

    private IEnumerator Shake()
    {
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        perlinChannel.m_AmplitudeGain = 0;
    }

    public void OnClickShake()
    {
        if(FindObjectOfType<CursorController>().IsClickSourceUnique())
        {
            Debug.Log("SHAKING!");
            SantardCameraShake(4.0f, 3.0f, 1, 0);
        }       
    }
}
