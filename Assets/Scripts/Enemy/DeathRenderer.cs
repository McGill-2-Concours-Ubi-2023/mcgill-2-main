using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class DeathRenderer : MonoBehaviour
{
    public Renderer[] _renderers;
    public Material deathMaterial;
    [Range(1.0f, 10.0f)]
    public float dissolveTime = 2.0f;
    public Animator animator;
    public VisualEffect deathParticles;
    public NavMeshAgent agent;
    public EnemyAI ai;
    public Enemy enemy;
    public Gun1 gun1;
    public Gun gun;
    public Rigidbody rb;
    public Collider[] colliders;

    public void OnDeathRender()
    {
        DisableColliders();
        FreezeOnCurrentState();
        foreach (Renderer renderer in _renderers)
        {
            renderer.material = deathMaterial;
        }
        StartCoroutine(Dissolve(dissolveTime));
    }

    private IEnumerator Dissolve(float dissolveTime)
    {       
        deathParticles.SetFloat("DissolveDuration", dissolveTime / 3);
        deathParticles.SendEvent("OnDeath");
        float elapsedTime = 0;
        while (elapsedTime < dissolveTime)
        {
            float t = elapsedTime / dissolveTime;
            float threshold = Mathf.Lerp(0, 1.5f, t);
            foreach(Renderer renderer in _renderers)
            {
                renderer.material.SetFloat("_Dissolve_threshold", threshold);
            }                  
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(transform.root.gameObject);
    }

    private void FreezeOnCurrentState()
    {
        ComponentsFreeze();
        if (agent)
        {
            agent.speed = 0;
            agent.angularSpeed = 0;
            agent.acceleration = 0;
            agent.isStopped = true;
        }
        if(animator != null)
        animator.enabled = false;
    }

    public void EnableComponents()
    {
        if (gun) gun.enabled = true;
        if (gun1) gun1.enabled = true;
        if (ai) ai.enabled = true;
        if (enemy) enemy.enabled = true;
    }

    public void ComponentsFreeze()
    {
        if (gun) gun.enabled = false;
        if (gun1) gun1.enabled = false;
        if (ai) ai.enabled = false;
        if (enemy) enemy.enabled = false;
    }

    private void DisableColliders()
    {
        //if the object has a rigidBody attached to it, disable gravity before disable colliders
        rb.constraints = RigidbodyConstraints.FreezeAll;
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
