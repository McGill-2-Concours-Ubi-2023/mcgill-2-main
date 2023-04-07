using Unity.Mathematics;

public interface IMainCharacterTriggers : ITrigger
{
    public void CollectCoin() { }
    
    public void SetNavActionData(object data) { }

    public void GetNavActionData(Ref<object> outData) { }
    
    public void OnInput(float2 input) { }
    
    public void OnMovementIntention(float3 intention) { }
    
    public void OnDashIntention() { }
    
    public void OnDebugCameraRotation(float2 input) { }
    
    public void OnPlayerFaceIntention(float3 intention) { }
    
    public void OnSpawnCrateIntention() { }
    
    public void HasFaceDirectionInput(Ref<bool> hasInput) { }
    
    public void AdjustFaceDirection(float3 direction) { }
    
    public void UpdateMovementDirection(Ref<float3> direction) { }
    
    public void IsDashing(Ref<bool> refIsDashing) { }
    
    public void OnRoomCleared() { }
    public void OnPrimaryWeaponRelease() { }
    
    public void ResetInventory() { }
    
    public void OnAutoFaceIntention(float3 intention) { }
}
