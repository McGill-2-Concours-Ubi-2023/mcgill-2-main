using Unity.Mathematics;
using UnityEngine;

public class MainCharacterDebugState : GenericStateMachineBehaviour<MainCharacterDebugStateBehaviour>
{
}

public class MainCharacterDebugStateBehaviour : GenericStateMachineMonoBehaviour, IMainCharacterTriggers
{
    private const float DebugMovementSpeed = 5f;
    private MainCharacterController m_Controller;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Controller = GetComponent<MainCharacterController>();
        m_Controller.DebugCamera.gameObject.SetActive(true);
        m_Controller.Camera.gameObject.SetActive(false);
    }

    public void OnMovementIntention(float3 intention)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = intention * DebugMovementSpeed;
        Debug.DrawRay(transform.position, intention * DebugMovementSpeed, Color.red, 0.1f);
    }

    public void OnDebugCameraRotation(float2 input)
    {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Controller.Camera.gameObject.SetActive(true);
        m_Controller.DebugCamera.gameObject.SetActive(false);
    }
}

