using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
public interface IVibrationTrigger : ITrigger
{
    void StartRumbling();
    void StopRumbling();

}
public abstract class GravityField : MonoBehaviour
{
    [SerializeField]
    private GameObject destructionBounds;
    [Range(0.1f, 9.81f)]
    public float gravity;
    public HashSet<GameObject> agents;
    protected bool isActive;
    protected int fieldMask;
    protected int collisionMask;
    [SerializeField]
    [Range(0.5f, 3.0f)]
    protected float massCompression = 1.0f;
    protected List<Rigidbody> cachedGenericBodies;
    protected List<Rigidbody> cachedBulletBodies;
    public static int maxNumAgents = 500;
    private GravityAgentSample bulletsSample;
    private GravityAgentSample genericSample;
    private float fieldRadius;
    private Animator playerAnimator;
    private Rigidbody bufferRb;
    private GravityAgent bufferGravityAgent;
    [SerializeField]
    GameObject vib;
    protected abstract void ApplyGravity(Rigidbody rb);

    public void SetActive(bool active)
    {
        //RefreshCollider();
        UpdateCollisionMask();
        if (agents == null) agents = new HashSet<GameObject>();
        if (cachedGenericBodies == null) cachedGenericBodies = new List<Rigidbody>();
        if (cachedBulletBodies == null) cachedBulletBodies = new List<Rigidbody>();
        fieldMask = GlobalAgent.playerMask;//add masks to ignore the field's rotation correction
        isActive = active;
        StartCoroutine(CheckCollidersOverTime(1.0f));
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void RefreshCollider()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false; // disable the collider
        collider.enabled = true; // re-enable the collider
    }

    public void UpdateCollisionMask()
    {
        int cubesMask = 1 << LayerMask.NameToLayer("BossCubes");
        int playerLayer = 1 << GlobalAgent.playerMask;
        int playerBulletLayer = 1 << GlobalAgent.playerBulletMask;
        int enemyBulletLayer = 1 << GlobalAgent.enemyBulletMask;
        int destructibleLayer = 1 << Destructible.desctructibleMask;
        collisionMask = playerLayer | playerBulletLayer | enemyBulletLayer | destructibleLayer | cubesMask;
    }

    public void SetMassCompressionForce(float compressionForce)
    {
        massCompression = compressionForce;
    }

    public float GetMassCompressionForce()
    {
        return massCompression;
    }

    //collision condition set in collision matrix go to Edit > Project settings > Layer collision matrix
    public void ProcessCollision(Collider other)
    {
        if (isActive)
        {
            // Use cached components instead of calling GetComponent every time
            bufferRb = other.GetComponent<Rigidbody>();
            if (bufferRb.isKinematic) bufferRb.isKinematic = false;
            bufferGravityAgent = other.GetComponent<GravityAgent>();
            if (!agents.Contains(other.gameObject) && bufferGravityAgent != null)
            {

                if(bufferRb.CompareTag("PlayerBullet") || bufferRb.CompareTag("EnemyBullet"))
                {
                    cachedBulletBodies.Add(bufferRb);
                } else
                {
                    cachedGenericBodies.Add(bufferRb);
                }
                agents.Add(other.gameObject);
                bufferGravityAgent.BindField(this);                
            }

            if(other.CompareTag("Player"))
            {
                playerAnimator.SetBool("IsFloating", true);
                vib.Trigger<IVibrationTrigger>(nameof(IVibrationTrigger.StartRumbling));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {       
        ProcessCollision(other);
    }

    private IEnumerator CheckCollidersOverTime(float timeToCheck)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, fieldRadius, collisionMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (elapsedTime >= timeToCheck)
            {
                yield break;
            }
            if(colliders[i] != null)
            ProcessCollision(colliders[i]);
            elapsedTime = Time.time - startTime;
            yield return null;
        }
    }

    private void OnEnable()
    {
        fieldRadius = GetComponent<SphereCollider>().radius;
        genericSample = new GravityAgentSample(maxNumAgents, Allocator.Persistent);
        bulletsSample = new GravityAgentSample(maxNumAgents, Allocator.Persistent);
        vib = GameObject.Find("GamepadVib"); 
    }

    private void FixedUpdate()
    {
        HandleGravity();       
    }

    private void HandleGravity()
    {
        ConcentricGravityField field = this as ConcentricGravityField;

        bulletsSample.UpdateSample(cachedBulletBodies, RotationAxis.Forward);
        JobHandle handle1 = bulletsSample.InitializeGravityJob(Time.deltaTime, cachedBulletBodies.Count, 20,
            transform.position, field.orientationSpeed, field.mediumDensity, field.attractionForce,
            field.gravity, field.dampeningForce, BulletAgent.bulletBendingForce, true);

        genericSample.UpdateSample(cachedGenericBodies, RotationAxis.Up);
        JobHandle handle2 = genericSample.InitializeGravityJob(Time.deltaTime, cachedGenericBodies.Count, 20,
            transform.position, field.orientationSpeed, field.mediumDensity, field.attractionForce,
            field.gravity, field.dampeningForce, GlobalAgent.externalForce, false);

        handle1.Complete();
        handle2.Complete();

        bulletsSample.RetrieveData(cachedBulletBodies);
        genericSample.RetrieveData(cachedGenericBodies);
    }

    private void OnTriggerExit(Collider other)
    {
        ReleaseAgent(other.gameObject);
        if (other.CompareTag("Player"))
        {
            vib.Trigger<IVibrationTrigger>(nameof(IVibrationTrigger.StopRumbling));
        }
    }

    public void ReleaseAgent(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            obj.GetComponent<Animator>().SetBool("IsFloating", false);
        }
        agents.Remove(obj);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            cachedGenericBodies.Remove(rb);
        }
    }

    private void OnDisable()
    {
        genericSample.Dispose();
        bulletsSample.Dispose();
    }

    private void OnDestroy()
    {
        vib.Trigger<IVibrationTrigger>(nameof(IVibrationTrigger.StopRumbling));
        foreach (var rb in cachedGenericBodies)
        {
            if(rb != null)
            {
                GravityAgent agent = rb.GetComponent<GravityAgent>();
                agent.Release();
                agent.TryStopDestroy();
            }
        }
        try
        {
            GameObject.Find("MainCharacter")
           .GetComponent<Animator>()
           .SetBool("IsFloating", false);
            cachedGenericBodies.Clear();
            cachedBulletBodies.Clear();
        } catch
        {
            //Do nothing
        }

    }
    
}
