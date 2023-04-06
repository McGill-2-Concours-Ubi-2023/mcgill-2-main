using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVibration : MonoBehaviour
{
    public interface IVibrationTrigger : ITrigger
    {
        void StartRumbling();
        void StopRumbling();

    }
}
