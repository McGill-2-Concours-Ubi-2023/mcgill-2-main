using UnityEngine;

public class MainCharacterFreeFallCheckAction : INavAction
{
    public bool isGrounded;
    
    public bool ShouldTransition(GameObject gameObject)
    {
        bool grounded = Physics.CheckSphere(gameObject.transform.position, 0.1f);
        isGrounded = grounded;
        return !grounded;
    }
}

