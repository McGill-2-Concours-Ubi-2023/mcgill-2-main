using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineCameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;
    private CinemachineBasicMultiChannelPerlin perlinChannel;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlinChannel = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void ShakeCamera(float intensity, float timer)
    {      
        perlinChannel.m_AmplitudeGain = intensity;
        shakeTimer = timer;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
        }
        else 
        {
            perlinChannel.m_AmplitudeGain = 0;
        }
    }

    public void OnClickShake()
    {
        if(!Gamepad.current.aButton.IsPressed() && !Mouse.current.leftButton.IsPressed())
        {
            Debug.Log("SHAKING!");
            ShakeCamera(4.0f, 3.0f);
        }       
    }
}
