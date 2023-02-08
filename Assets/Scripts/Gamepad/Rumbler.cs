using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumbler : MonoBehaviour
{
    private Gamepad gamepad;
    private float vibrationDuration;
    private float vibrationTimer;
    private float leftVibrationStrength;
    private float rightVibrationStrength;

    void OnEnable()
    {
        gamepad = Gamepad.current;
    }

    void Update()
    {
        if (vibrationTimer > 0)
        {
            gamepad.SetMotorSpeeds(leftVibrationStrength, rightVibrationStrength);
            vibrationTimer -= Time.deltaTime;
        }
        else
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

    public void TriggerVibration(float duration, float leftStrength, float rightStrength)
    {
        vibrationDuration = duration;
        vibrationTimer = duration;
        leftVibrationStrength = leftStrength;
        rightVibrationStrength = rightStrength;
    }
}
