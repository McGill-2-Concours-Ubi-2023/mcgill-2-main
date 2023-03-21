using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GravitationalGrenade : MonoBehaviour
{
    [SerializeField] [Range(0, 10)]
    private float destructionTimer;
    [SerializeField][Range(0, 5)]
    private float fieldVerticalOffset = 0;
    private Animator animator;
    private GravityField gravityField;
    [SerializeField]
    private GameObject explodeEffectPrefab;
    [SerializeField]
    private Material mainMaterial;
    private MeshRenderer meshRenderer;
    [SerializeField][ColorUsage(true, true)]
    private Color defaultColor;
    [SerializeField][ColorUsage(true, true)]
    private Color explosionColor;
    private Renderer _renderer;
    private bool hasExploded = false;
    [SerializeField][Range(0.5f, 10.0f)]
    private float explosionShakeIntensity = 0.1f;
    [SerializeField]
    private float explosionShakeTime = 0.5f;
    [SerializeField]
    private float explosionInfluenceDistance = 10.0f;
    [SerializeField]
    private float wobbleShakeIntensity = 4.0f;
    [SerializeField]
    private float shakeDampening = 6.0f;
    [SerializeField][Range(0.1f, 0.005f)]
    private float particlesFadeRate = 0.1f;
    private VisualEffect _explodeEffect;
    [SerializeField]
    private GameObject swarmEffectPrefab;
    private VisualEffect swarmEffect;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gravityField = GetComponentInChildren<GravityField>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = mainMaterial;
        gravityField.SetActive(false);
        _renderer = transform.Find("SphereMesh").GetComponent<Renderer>();
        _renderer.material.SetColor("_BaseColor", defaultColor);
    }

    public VisualEffect GetVisualEffect()
    {
        return explodeEffectPrefab.GetComponent<VisualEffect>();
    }

    public float GetDestructionTimer() { return destructionTimer; }

    public void Activate()
    {
        animator.SetTrigger("activate");
    }

    public void StartSwarm()
    {
        GameObject obj = Instantiate(swarmEffectPrefab, transform.position, Quaternion.identity);
        swarmEffect = obj.GetComponent<VisualEffect>();
        swarmEffect.playRate = 2.5f;
        swarmEffect.SendEvent("OnSwarmPlay");
    }

    public void StopSwarm()
    {
        swarmEffect.SendEvent("OnSwarmStop");
    }

    public void Explode() //"Spawns" the gravity field object
    {
        if (!hasExploded)
        {
            float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
            var intensity = Mathf.Lerp(0.1f, explosionShakeIntensity, 1 - Mathf.Clamp01(distance / explosionInfluenceDistance));
            _renderer.material.SetColor("_BaseColor", explosionColor);
            animator.SetTrigger("explode");
            GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger, float, float, float>
                (nameof(IGravityToCameraTrigger.OnCameraStandardShake), intensity, explosionShakeTime, 1);
            var kernel = gravityField.transform.position;
            //Slightly offset the y position of the field's kernel for better physics
            gravityField.transform.position = new Vector3(kernel.x, kernel.y + fieldVerticalOffset, kernel.z);
            gravityField.SetActive(true);
            GameObject obj = Instantiate(explodeEffectPrefab, transform.position, Quaternion.identity);
            _explodeEffect = obj.GetComponent<VisualEffect>();
            _explodeEffect.SetFloat("Field radius", ((ConcentricGravityField)gravityField).Radius());
            //meshRenderer.material = explosionMaterial;
            transform.Find("SphereMesh").GetComponent<SphereCollider>().enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
            StartCoroutine(InitializeVFX());
            StartCoroutine(ShakeCameraGravity());
            hasExploded = true;
        }      
    }

    private IEnumerator ShakeCameraGravity()
    {
        while (true)
        {
            float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
            var intensity = Mathf.Lerp(0.1f, wobbleShakeIntensity, 1 - Mathf.Clamp01(distance / explosionInfluenceDistance));
            var frequencyGain = intensity / (shakeDampening * 2);
            GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger, float, float>
               (nameof(IGravityToCameraTrigger.OnCameraWobbleShakeManualDecrement), intensity / shakeDampening, frequencyGain);
            yield return new WaitForEndOfFrame();
        }        
    }

    private IEnumerator InitializeVFX()
    {
        yield return new WaitForEndOfFrame();
        _explodeEffect.SendEvent("OnBurst");
        yield return new WaitForSeconds(0.1f);
        _explodeEffect.SendEvent("OnLoop");
    }

    public void DisappearOverTime(float timer)
    {
        StartCoroutine(Despawn(timer));  
    }

    private IEnumerator StopParticles()
    {
        while(_explodeEffect.GetFloat("Alpha") > 0)
        {
            _explodeEffect.SetFloat("Alpha", _explodeEffect.GetFloat("Alpha") - particlesFadeRate);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }

    private IEnumerator Despawn(float timer)
    {
        
        float elapsedTime = 0.0f;
        if(timer < 5.0f) _explodeEffect.Stop();
        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            if (timer - elapsedTime < 5.0f) _explodeEffect.Stop();
            yield return new WaitForEndOfFrame();
        }
        _explodeEffect.SendEvent("OnStop");
        animator.SetTrigger("despawn");
        swarmEffect.SendEvent("OnSwarmStop");
        gravityField.SetActive(false);
    }

    public void SelfDestroy() //triggered in animator
    {
        StopAllCoroutines();
        GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger>
              (nameof(IGravityToCameraTrigger.StopCameraShake));
        StartCoroutine(StopParticles());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
