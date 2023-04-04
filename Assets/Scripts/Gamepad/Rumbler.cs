using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumbler : MonoBehaviour
{
    private Gamepad gamepad;
    private Coroutine rumbleCoroutine;
    private List<VibrationRequest> vibrationRequests = new List<VibrationRequest>();
    private float leftVibrationStrength;
    private float rightVibrationStrength;

    private void Update()
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
        }

        if (gamepad != null)
        {
            // Update all active vibration requests
            for (int i = vibrationRequests.Count - 1; i >= 0; i--)
            {
                VibrationRequest request = vibrationRequests[i];
                if (request.IsComplete())
                {
                    vibrationRequests.RemoveAt(i);
                }
                else
                {
                    request.Update(gamepad);
                }
            }
        }
    }

    public void TriggerVibration(float duration, float leftStrength, float rightStrength)
    {
        gamepad = Gamepad.current;
        if (gamepad != null)
        {
            // stop any previous rumble coroutine
            if (rumbleCoroutine != null)
            {
                StopCoroutine(rumbleCoroutine);
            }

            // set new vibration strengths
            leftVibrationStrength = leftStrength;
            rightVibrationStrength = rightStrength;

            // start new rumble coroutine
            rumbleCoroutine = StartCoroutine(StopRumble(duration, gamepad));
            gamepad.SetMotorSpeeds(leftStrength, rightStrength);
        }
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        yield return new WaitForSeconds(duration);

        pad.SetMotorSpeeds(0f, 0f);

        // reset rumble coroutine flag
        rumbleCoroutine = null;
    }

    private class VibrationRequest
    {
        private float startTime;
        private float duration;
        private float leftStrength;
        private float rightStrength;

        public VibrationRequest(float duration, float leftStrength, float rightStrength)
        {
            this.startTime = Time.time;
            this.duration = duration;
            this.leftStrength = leftStrength;
            this.rightStrength = rightStrength;
        }

        public void Update(Gamepad gamepad)
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float strength = Mathf.Lerp(0f, leftStrength, t);
            gamepad.SetMotorSpeeds(strength, strength);
        }

        public bool IsComplete()
        {
            return Time.time >= startTime + duration;
        }
    }
}