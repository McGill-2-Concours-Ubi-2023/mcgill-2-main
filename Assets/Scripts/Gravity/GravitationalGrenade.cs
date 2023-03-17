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
    private VisualEffect _explodeEffect;
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
        return _explodeEffect;
    }

    public float GetDestructionTimer() { return destructionTimer; }

    public void Activate()
    {
        animator.SetTrigger("activate");
    }

    public void Explode() //"Spawns" the gravity field object
    {
        if (!hasExploded)
        {
            float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
            var intensity = Mathf.Lerp(0.1f, explosionShakeIntensity, 1 - Mathf.Clamp01(distance / explosionInfluenceDistance));
            _renderer.material.SetColor("_BaseColor", explosionColor);
            animator.SetTrigger("explode");
            GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger>
                (nameof(IGravityToCameraTrigger.OnCameraStandardShake), intensity, explosionShakeTime, 1);
            var kernel = gravityField.transform.position;
            //Slightly offset the y position of the field's kernel for better physics
            gravityField.transform.position = new Vector3(kernel.x, kernel.y + fieldVerticalOffset, kernel.z);
            gravityField.SetActive(true);
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
            GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger>
               (nameof(IGravityToCameraTrigger.OnCameraWobbleShakeManualDecrement), intensity / shakeDampening, frequencyGain);
            yield return new WaitForEndOfFrame();
        }        
    }

    private IEnumerator InitializeVFX()
    {
        yield return new WaitForEndOfFrame();
        _explodeEffect.SendEvent("OnBurst");
        yield return new WaitForSeconds(0.5f);
        _explodeEffect.SendEvent("OnLoop");
    }

    public void DisappearOverTime(float timer)
    {
        StartCoroutine(StopParticles(timer));       
    }

    private IEnumerator StopParticles(float timer)
    {
        yield return new WaitForSeconds(timer);
        _explodeEffect.Stop();
        StartCoroutine(Despawn());       
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(6.0f); //Life time of particle system is random between 1 and 5
        //GameObject mesh = transform.Find("SphereMesh").gameObject;
        animator.SetTrigger("despawn");
    }

    public void SelfDestroy()
    {
        StopAllCoroutines();
        GameObject.FindGameObjectWithTag("Player").Trigger<IGravityToCameraTrigger>
              (nameof(IGravityToCameraTrigger.StopCameraShake));
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
