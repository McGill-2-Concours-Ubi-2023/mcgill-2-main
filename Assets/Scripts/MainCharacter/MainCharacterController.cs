using System;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterController : MonoBehaviour, IMainCharacterTriggers
{
    public float MovementSpeed;
    public float DashSpeed;
    public CinemachineVirtualCamera Camera;
    private object m_NavActionData;
    public CinemachineVirtualCamera DebugCamera;

    private ISimpleInventory<SimpleCollectible> m_SimpleCollectibleInventory;
    
    private InputActionAsset m_InputActionAsset;
    private readonly static int InDebugMode = Animator.StringToHash("InDebugMode");

    private void Awake()
    {
        m_SimpleCollectibleInventory = new SimpleInventory<SimpleCollectible>();
        m_InputActionAsset = GetComponent<PlayerInput>().actions;
    }

    private void Update()
    {
        float2 input = m_InputActionAsset["Movement"].ReadValue<Vector2>();
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnInput), input);

        float3 adjustedInput = new float3();
        adjustedInput.xz = input.xy;

        CinemachineVirtualCamera cam = GetActiveCamera();

        float3 cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float3 cameraRight = cam.transform.right;
        float3 adjustedDirection = adjustedInput.x * cameraRight + adjustedInput.z * cameraForward;

        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnMovementIntention), adjustedDirection);

        #if DEBUG
        float2 camInput = m_InputActionAsset["CameraMove"].ReadValue<Vector2>();
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDebugCameraRotation), camInput);
        #endif
    }

#if DEBUG
    public void OnDebug()
    {
        Debug.Log("Toggle debug mode");
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.SetBool(InDebugMode, !animator.GetBool(InDebugMode));
        }
    }
#endif
    
    public void CollectCoin()
    {
        m_SimpleCollectibleInventory.AddItem(SimpleCollectible.Coin);
    }

    public void SetNavActionData(object data)
    {
        m_NavActionData = data;
    }

    public void GetNavActionData(Ref<object> outData)
    {
        outData.Value = m_NavActionData;
    }

    public void OnDash()
    {
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDashIntention));
    }
    
    public CinemachineVirtualCamera GetActiveCamera()
    {
        return DebugCamera.gameObject.activeSelf ? DebugCamera : Camera;
    }
}

