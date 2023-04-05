public interface IGravityToCameraTrigger : ITrigger
{
    public void OnCameraStandardShake(float intensity, float timer, float frequencyGain);

    public void OnCameraWobbleShakeManualDecrement(float intensity, float frequencyGain);

    public void StopCameraShake();
}
