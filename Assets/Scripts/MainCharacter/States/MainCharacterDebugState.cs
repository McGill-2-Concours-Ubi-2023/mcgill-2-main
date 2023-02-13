using Cinemachine;
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
        Rigidbody rb = GetComponent<Rigidbody>();
        if (input.x != 0)
        {
            gameObject.transform.Rotate(Vector3.up, input.x * 90 * Time.deltaTime);
            rb.angularVelocity = Vector3.zero;
        }
        
        if (input.y != 0)
        {
            Transform cameraBaseTransform = m_Controller.DebugCamera.Follow;
            cameraBaseTransform.Rotate(-Vector3.right, input.y * 90 * Time.deltaTime);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Controller.Camera.gameObject.SetActive(true);
        m_Controller.DebugCamera.gameObject.SetActive(false);
    }
}

