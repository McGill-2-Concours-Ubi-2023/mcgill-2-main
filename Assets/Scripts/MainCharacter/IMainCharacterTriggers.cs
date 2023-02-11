using Unity.Mathematics;

public interface IMainCharacterTriggers : ITrigger
{
    public void CollectCoin() { }
    
    public void SetNavActionData(object data) { }

    public void GetNavActionData(Ref<object> outData) { }
    
    public void OnInput(float2 input) { }
    
    public void OnMovementIntention(float3 intention) { }
    
    public void OnDashIntention() { }
}
