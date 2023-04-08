using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using float3 = Unity.Mathematics.float3;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class MainCharacterController : MonoBehaviour, IMainCharacterTriggers, ICrateTriggers, IGravityToCameraTrigger
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
    public float doorOpenRate = 1f;
    private Animator animator;
    private bool inputState = false;
    public Coroutine danceCoroutine;
    private Rigidbody rb;
    private bool isDashing;
    [Range(0, 1)]
    public float dashDuration = 0.3f;
    private bool m_GamePaused;
    [CanBeNull]
    private GameObject m_PauseMenu;
    public VisualEffect trailFollowEffect;
    private Health health;
    public bool startFight;
    private int m_RoomClearedCount;

    public ISimpleInventory<SimpleCollectible> SimpleCollectibleInventory;
    
    private InputActionAsset m_InputActionAsset;
    private static readonly int InDebugMode = Animator.StringToHash("InDebugMode");
    private Vibration vibration; 
    private GrenadeCrateUI gcUI;
    
    public ClickSound cs;
    public AudioClip dashSound;
    private bool m_Awake = true;

    private async void Awake()
    {
        m_Awake = true;
        Transform cameraRoot = transform.Find("CameraRoot");
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BossScene"))
        {
            this.Camera.Follow = cameraRoot;
            this.Camera.LookAt = cameraRoot;
            this.Camera.m_Lens.FieldOfView = 25.0f;
        }
        vibration = GameObject.Find("GamepadVib").GetComponent<Vibration>();
            rb = GetComponent<Rigidbody>();
        SimpleCollectibleInventory = new SimpleInventory<SimpleCollectible>(new Dictionary<SimpleCollectible, int>
        {
            { SimpleCollectible.Grenade, 8 },
            { SimpleCollectible.CratePoint, 20 }
        });
        {
            using HSimpleInventoryLockGuard inventoryLockGuard = SimpleCollectibleInventory.Lock();
            m_InputActionAsset = GetComponent<PlayerInput>().actions;
            animator = GetComponent<Animator>();
            m_PauseMenu = GameObject.FindWithTag("PauseMenu");
            health = GetComponent<Health>();
            health.OnDeath += OnPlayerDeath;
            if (m_PauseMenu)
            {
                m_PauseMenu.SetActive(false);
            }
            gcUI = GameObject.FindObjectOfType<GrenadeCrateUI>();
            cs = GetComponent<ClickSound>();
            while (GameManager.isLoading)
            {
                await Task.Yield();
            }
        }
        m_Awake = false;
    }

    public void StartFight()
    {
        Transform targetGroup = GameObject.Find("CinemachineTargetGroup").transform;
        this.Camera.Follow = targetGroup;
        this.Camera.LookAt = targetGroup;
        this.Camera.m_Lens.FieldOfView = 47.0f;
    }

    private void OnPlayerDeath()
    {
        SceneManager.LoadScene("Menu");
    }

    private async void Start()
    {
        StartCoroutine(RandomDance());
        while (GameManager.isLoading)
        {
            await Task.Yield();
        }
        while (m_Awake)
        {
            await Task.Yield();
        }
        SimpleCollectibleInventory.AddInBulk(SimpleCollectible.Grenade, 2);
        SimpleCollectibleInventory.AddInBulk(SimpleCollectible.CratePoint, 5);
        UpdateInventoryUI();
    }

    IEnumerator RandomDance()
    {
        while (true)
        {
            int randomEventTrigger = UnityEngine.Random.Range(6, 10);
            yield return new WaitForSeconds(randomEventTrigger);
            if(danceCoroutine != null)StopCoroutine(danceCoroutine);
            danceCoroutine = StartCoroutine(OnInputWait());
        }
    }

    IEnumerator OnInputWait()
    {
        if (!inputState) 
        {
            float randomWait = UnityEngine.Random.Range(2, 4);
            while(randomWait > 0)
            {
                randomWait -= Time.deltaTime;
                if (inputState || animator.GetBool("IsFloating") && danceCoroutine != null)
                    StopCoroutine(danceCoroutine);
                yield return new WaitForEndOfFrame();
            }
            if (!inputState)
            {
                int randomNumber = UnityEngine.Random.Range(1, 3);
                animator.SetTrigger("Dance_" + randomNumber);
            }           
        }
    }


    public void OnDanceEvent()
    {
        //if(danceCoroutine != null)StopCoroutine(danceCoroutine);
    }

    private void Update()
    {
        //gcUI.UpdateGrenadeUI(SimpleCollectibleInventory.GetCount(SimpleCollectible.Grenade));
        gcUI.UpdateCrateUI(SimpleCollectibleInventory.GetCount(SimpleCollectible.CratePoint));
        if (startFight) StartFight();
        float2 input = m_InputActionAsset["Movement"].ReadValue<Vector2>();
        gameObject.Trigger<IMainCharacterTriggers, float2>(nameof(IMainCharacterTriggers.OnInput), input);
        float3 adjustedInput;

        if (!isDashing)
        {
            adjustedInput = new float3
            {
                xz = input.xy
            };
        }
        else adjustedInput = float3.zero;

        inputState = (new Vector2(input.x, input.y)).magnitude > 0.05f;
        animator.SetBool("InputState", inputState);
        if (inputState) animator.SetBool("IsFloating", false);

        CinemachineVirtualCameraBase cam = GetActiveCamera();

        float3 cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float3 cameraRight = cam.transform.right;
        float3 adjustedDirection = adjustedInput.x * cameraRight + adjustedInput.z * cameraForward;

        gameObject.Trigger<IMainCharacterTriggers, float3>(nameof(IMainCharacterTriggers.OnMovementIntention), adjustedDirection);

        float2 rightStick = m_InputActionAsset["CameraMove"].ReadValue<Vector2>();
        #if DEBUG
        gameObject.Trigger<IMainCharacterTriggers, float2>(nameof(IMainCharacterTriggers.OnDebugCameraRotation), rightStick);
        #endif

        float3 adjustedFaceInput = new float3
        {
            xz = rightStick.xy
        };
        float3 adjustedFaceDirection = adjustedFaceInput.x * cameraRight + adjustedFaceInput.z * cameraForward;
        gameObject.Trigger<IMainCharacterTriggers, float3>(nameof(IMainCharacterTriggers.OnPlayerFaceIntention), adjustedFaceDirection);

        m_MovementDirection = normalize(all(adjustedInput.xz == float2.zero) ? transform.forward : adjustedDirection);
        Debug.DrawRay(transform.position + Vector3.up * 3, m_MovementDirection, Color.magenta);
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
                var targetTransform = m_LastRoom.transform.Find("cameraRoot");
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

    public void ActivateTrail()
    {
        trailFollowEffect.SendEvent("OnFollowTrail");
    }

    public void StopTrail()
    {
        trailFollowEffect.SendEvent("OnStopTrail");
    }

    public void OnDash()
    {
        //gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDashIntention));
        GetComponent<DashMeshTrail>().ActivateTrail();
        if (!isDashing)
        {
            cs.Click(dashSound);
            animator.SetTrigger("MovementToDash");
            StartCoroutine(Dash());
        }
        vibration.SoftVibration();
    }

    public void StopDash() //triggered as an animation event at the last keyframe
    {
        animator.SetTrigger("StopDash");
    }

    IEnumerator Dash()
    {

        isDashing = true;
        float3 dashDirection = m_MovementDirection;
        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.velocity = (Vector3)dashDirection * DashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }
        rb.velocity = rb.velocity / 5;
        isDashing = false;

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
        try
        {
            SimpleCollectibleInventory.RemoveItem(SimpleCollectible.Grenade);

        }
        catch (InventoryEmptyException<SimpleCollectible> e)
        {
            Debug.Log("No grenades left");
            return;
        }
        int grenadeNum = SimpleCollectibleInventory.GetCount(SimpleCollectible.Grenade); 
        Debug.Log($"{grenadeNum} grenades left");
        gcUI.UpdateGrenadeUI(grenadeNum);
        int randomNumber = UnityEngine.Random.Range(1, 3);
        animator.SetTrigger("ThrowGrenade_" + randomNumber);
        GameObject grenade = Instantiate(GravityGrenadePrefab);
        float3 throwDir = (transform.forward + transform.up * 0.5f).normalized;
        Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), grenade.transform.Find("SphereMesh").GetComponent<SphereCollider>());
        grenade.transform.position = transform.position + Vector3.up * 0.5f;
        Rigidbody grenade_rb = grenade.GetComponent<Rigidbody>();
        grenade_rb.AddForce(throwDir * 8, ForceMode.Impulse);
        StartCoroutine(GrenadeDelayedExplode(grenade));
    }

    public void OnCollectCrate()
    {
        SimpleCollectibleInventory.AddItem(SimpleCollectible.CratePoint);
        int crateNum = SimpleCollectibleInventory.GetCount(SimpleCollectible.CratePoint);
        gcUI.UpdateCrateUI(crateNum);
    }

    public void OnSpawnCrate()
    {
        gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnSpawnCrateIntention));
        int crateNum = SimpleCollectibleInventory.GetCount(SimpleCollectible.CratePoint);
        gcUI.UpdateCrateUI(crateNum);
    }

    public void FreezeOnCurrentState()
    {
        //Freeze player
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreeze()
    {
        //Unfreeze player
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
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
        
        rb.rotation = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);
        rb.angularVelocity = Vector3.zero;
    }

    public void UpdateMovementDirection(Ref<float3> direction)
    {
        direction.Value = m_MovementDirection;
    }

    public Vector3 GetMovementDirection()
    {
        return (Vector3)m_MovementDirection;
    }

    private IEnumerator CheckForTurn()
    {
        while (true)
        {
            var previousDirection = m_MovementDirection;
            yield return new WaitForSeconds(0.1f);
            var rotation = Quaternion.FromToRotation(m_MovementDirection, previousDirection);
            if(rotation.eulerAngles.y > 160f)
            {
                animator.SetTrigger("Turn");
            }
        }       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(1);
        }
    }

    public void OnCameraStandardShake(float intensity, float timer, float frequencyGain)
    {
        Camera.GetComponent<CinemachineCameraShake>().SantardCameraShake(intensity, timer, 1, 0);
    }

    public void OnCameraWobbleShakeManualDecrement(float intensity, float frequencyGain)
    {
        Camera.GetComponent<CinemachineCameraShake>().WobbleGravityShake(intensity, frequencyGain, 1);
    }

    public void StopCameraShake()
    {
        Camera.GetComponent<CinemachineCameraShake>().StopCameraShake();
    }

    /*public void OnPause()
    {
        m_GamePaused = !m_GamePaused;
        Time.timeScale = m_GamePaused ? 0.0f : 1.0f;
        if (m_PauseMenu)
        {
            m_PauseMenu.SetActive(m_GamePaused);
        }
    }*/

    public void OnShootPress()
    {
        gameObject.TriggerDown<IGunTriggers>(nameof(IGunTriggers.OnShootStartIntention));
        
    }
    
    public void OnShootRelease()
    {
        gameObject.TriggerDown<IGunTriggers>(nameof(IGunTriggers.OnShootStopIntention));
    }
    
    public void IsDashing(Ref<bool> refIsDashing)
    {
        refIsDashing.Value = this.isDashing;
    }

    public void OnRoomCleared()
    {
        gcUI.UpdateCrateUI(SimpleCollectibleInventory.GetCount(SimpleCollectible.CratePoint));
        m_RoomClearedCount++;
        if (m_RoomClearedCount % 2 == 0)
        {
            SimpleCollectibleInventory.AddItem(SimpleCollectible.Grenade);
            SimpleCollectibleInventory.AddInBulk(SimpleCollectible.CratePoint, 5);
            UpdateInventoryUI();
        }
    }

    private void UpdateInventoryUI()
    {
        gcUI.UpdateGrenadeUI(SimpleCollectibleInventory.GetCount(SimpleCollectible.Grenade));
        gcUI.UpdateCrateUI(SimpleCollectibleInventory.GetCount(SimpleCollectible.CratePoint));
    }

    public void ResetInventory()
    {
        SimpleCollectibleInventory.ResetAll();
    }
}

