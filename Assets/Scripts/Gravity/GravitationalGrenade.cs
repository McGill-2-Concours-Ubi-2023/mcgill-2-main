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
    //[SerializeField]
    //private Material explosionMaterial;
    [SerializeField]
    private bool expandWithMass = false;
    private MeshRenderer meshRenderer;
    [SerializeField][ColorUsage(true, true)]
    private Color defaultColor;
    [SerializeField][ColorUsage(true, true)]
    private Color explosionColor;
    private Renderer _renderer;
    private bool hasExploded = false;
    
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
            _renderer.material.SetColor("_BaseColor", explosionColor);
            animator.SetTrigger("explode");
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
            hasExploded = true;
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
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
