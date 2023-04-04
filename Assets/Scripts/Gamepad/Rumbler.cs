using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumbler : MonoBehaviour
{
    private Gamepad gamepad;
   /* private float vibrationDuration;
    private float vibrationTimer;
    private float leftVibrationStrength;
    private float rightVibrationStrength;*/

   /* void OnEnable()
    {
        gamepad = Gamepad.current;
    }*/

   /* void Update()
    {
        if (gamepad != null)
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
    }
*/


    public void TriggerVibration(float duration, float leftStrength, float rightStrength)
    {
        gamepad = Gamepad.current;
        if (gamepad != null) {
            gamepad.SetMotorSpeeds(leftStrength, rightStrength);
            StartCoroutine(StopRumble(duration, gamepad));
        }
        /*leftVibrationStrength = leftStrength;
        rightVibrationStrength = rightStrength;
        vibrationDuration = duration;
        vibrationTimer = duration;*/
    }

    private IEnumerator StopRumble(float duration, Gamepad pad) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        pad.SetMotorSpeeds(0f, 0f);
    }
}
