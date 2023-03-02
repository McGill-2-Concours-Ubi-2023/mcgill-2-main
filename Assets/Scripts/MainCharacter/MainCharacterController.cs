using System;
using System.Collections;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

public class MainCharacterController : MonoBehaviour, IMainCharacterTriggers, ICrateTriggers
{
    public float MovementSpeed;
    public float DashSpeed;
    public CinemachineVirtualCamera Camera;
    private object m_NavActionData;
    public CinemachineVirtualCameraBase DebugCamera;
    public GameObject GravityGrenadePrefab;
    public float GravityGrenadeDisappearTime;
    public float GravityGrenadeExplodeTime;
    public GameObject CratePrefab;

    public ISimpleInventory<SimpleCollectible> SimpleCollectibleInventory;
    
    private InputActionAsset m_InputActionAsset;
    private readonly static int InDebugMode = Animator.StringToHash("InDebugMode");

    private void Awake()
    {
        SimpleCollectibleInventory = new SimpleInventory<SimpleCollectible>();
        m_InputActionAsset = GetComponent<PlayerInput>().actions;
    }

    private void Update()
    {
        float2 input = m_InputActionAsset["Movement"].ReadValue<Vector2>();
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnInput), input);

        float3 adjustedInput = new float3();
        adjustedInput.xz = input.xy;

        CinemachineVirtualCameraBase cam = GetActiveCamera();

        float3 cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float3 cameraRight = cam.transform.right;
        float3 adjustedDirection = adjustedInput.x * cameraRight + adjustedInput.z * cameraForward;

        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnMovementIntention), adjustedDirection);

        float2 rightStick = m_InputActionAsset["CameraMove"].ReadValue<Vector2>();

        #if DEBUG
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDebugCameraRotation), rightStick);
        #endif

        float3 adjustedFaceInput = new float3();
        adjustedFaceInput.xz = rightStick.xy;
        float3 adjustedFaceDirection = adjustedFaceInput.x * cameraRight + adjustedFaceInput.z * cameraForward;
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnPlayerFaceIntention), adjustedFaceDirection);
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
        SimpleCollectibleInventory.AddItem(SimpleCollectible.Coin);
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
    
    public CinemachineVirtualCameraBase GetActiveCamera()
    {
        return DebugCamera.gameObject.activeSelf ? DebugCamera : Camera;
    }

    public void OnPrimaryWeaponPress()
    {
        Debug.Log("PRESSING!");
    }
    
    private IEnumerator GrenadeDelayedExplode(GameObject grenade)
    {
        yield return new WaitForSeconds(GravityGrenadeExplodeTime);
        grenade.SendMessage("Explode");
        grenade.GetComponent<MeshRenderer>().enabled = false;
    }
    
    private IEnumerator GrenadeDelayedDespawn(GameObject grenade)
    {
        yield return new WaitForSeconds(GravityGrenadeDisappearTime);
        Destroy(grenade);
    }
    
    public void OnPrimaryWeaponRelease()
    {
        Debug.Log("RELEASING!!");
        GameObject grenade = Instantiate(GravityGrenadePrefab);
        float3 throwDir = (transform.forward + transform.up).normalized;
        Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), grenade.GetComponent<SphereCollider>());
        grenade.transform.position = transform.position;
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.velocity = GetComponent<Rigidbody>().velocity;
        rb.AddForce(throwDir * 10, ForceMode.Impulse);
        StartCoroutine(GrenadeDelayedExplode(grenade));
        StartCoroutine(GrenadeDelayedDespawn(grenade));
    }

    public void OnCollectCrate()
    {
        SimpleCollectibleInventory.AddItem(SimpleCollectible.CratePoint);
    }

    public void OnSpawnCrate()
    {
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnSpawnCrateIntention));
    }

    public void HasFaceDirectionInput(Ref<bool> hasInput)
    {
        hasInput.Value = m_InputActionAsset["CameraMove"].ReadValue<Vector2>() != Vector2.zero;
    }

    public void AdjustFaceDirection(float3 direction)
    {
        if (all(direction == float3.zero))
        {
            return;
        }
        
        direction = normalize(direction);
        float3 forwardVector = new float3(0, 0, 1);
        float angle = -acos(dot(direction, forwardVector));
        float3 cross = math.cross(direction, forwardVector);
        if (cross.y < 0)
        {
            angle = -angle;
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.rotation = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);
        rb.angularVelocity = Vector3.zero;
    }
}

