using System;
using System.Collections;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
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
    private DungeonRoom m_LastRoom = null;
    private float3 m_MovementDirection;

    public ISimpleInventory<SimpleCollectible> SimpleCollectibleInventory;
    
    private InputActionAsset m_InputActionAsset;
    private static readonly int InDebugMode = Animator.StringToHash("InDebugMode");

    private void Awake()
    {
        SimpleCollectibleInventory = new SimpleInventory<SimpleCollectible>();
        m_InputActionAsset = GetComponent<PlayerInput>().actions;
    }

    private void Update()
    {
        float2 input = m_InputActionAsset["Movement"].ReadValue<Vector2>();
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnInput), input);

        float3 adjustedInput = new float3
        {
            xz = input.xy
        };

        CinemachineVirtualCameraBase cam = GetActiveCamera();

        float3 cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float3 cameraRight = cam.transform.right;
        float3 adjustedDirection = adjustedInput.x * cameraRight + adjustedInput.z * cameraForward;

        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnMovementIntention), adjustedDirection);

        float2 rightStick = m_InputActionAsset["CameraMove"].ReadValue<Vector2>();
        #if DEBUG
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDebugCameraRotation), rightStick);
        #endif

        float3 adjustedFaceInput = new float3
        {
            xz = rightStick.xy
        };
        float3 adjustedFaceDirection = adjustedFaceInput.x * cameraRight + adjustedFaceInput.z * cameraForward;
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnPlayerFaceIntention), adjustedFaceDirection);

        m_MovementDirection = normalize(all(adjustedInput.xz == float2.zero) ? transform.forward : adjustedDirection);
        Debug.DrawRay(transform.position + Vector3.up, m_MovementDirection, Color.green);
        
        // update camera focus
        if (cam.gameObject == Camera.gameObject)
        {
            // update camera follow
            if (m_LastRoom != DungeonRoom.GetActiveRoom())
            {
                m_LastRoom = DungeonRoom.GetActiveRoom();
                GameObject newCamera = Instantiate(Camera.gameObject);
                newCamera.name = $"MainCamera_{Guid.NewGuid()}";
                newCamera.SetActive(false);
                CinemachineVirtualCamera newVirtualCamera = newCamera.GetComponent<CinemachineVirtualCamera>();
                var targetTransform = m_LastRoom.transform;
                newVirtualCamera.m_Follow = targetTransform;
                newVirtualCamera.m_LookAt = targetTransform;
                newCamera.SetActive(true);
                Camera.gameObject.SetActive(false);
                Destroy(Camera.gameObject, 10.0f);
                Camera = newVirtualCamera;
            }
        }
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
    
    private IEnumerator GrenadeDelayedExplode(GameObject grenade)
    {
        grenade.SendMessage("Activate");
        yield return new WaitForSeconds(GravityGrenadeExplodeTime);
        grenade.SendMessage("Explode");
        grenade.SendMessage("DisappearOverTime", GravityGrenadeDisappearTime);
    }
    
    public void OnPrimaryWeaponRelease()
    {
        GameObject grenade = Instantiate(GravityGrenadePrefab);
        float3 throwDir = (transform.forward + transform.up).normalized;
        Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), grenade.transform.Find("SphereMesh").GetComponent<SphereCollider>());
        grenade.transform.position = transform.position + Vector3.up;
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(throwDir * 10, ForceMode.Impulse);
        StartCoroutine(GrenadeDelayedExplode(grenade));
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

    public void GetMovementDirection(Ref<float3> direction)
    {
        direction.Value = m_MovementDirection;
    }
}

