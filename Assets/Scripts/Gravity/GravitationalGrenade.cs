using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalGrenade : MonoBehaviour
{
    [SerializeField] [Range(0, 10)]
    private float destructionTimer;
    [SerializeField][Range(1, 5)]
    private float fieldVerticalOffset = 1.0f;
    private Animator animator;
    private GravityField gravityField;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        gravityField = GetComponentInChildren<GravityField>();
        gravityField.SetActive(false);
    }

    public float GetDestructionTimer() { return destructionTimer; }

    public void Activate()
    {
        animator.SetTrigger("activate");
        //DO STUFF
    }

    public void Explode() //"Spawns" the gravity field object
    {
        animator.SetTrigger("explode");
        var kernel = gravityField.transform.position;
        //Slightly offset the y position of the field's kernel for better physics
        gravityField.transform.position = new Vector3(kernel.x, kernel.y + fieldVerticalOffset, kernel.z);
        gravityField.SetActive(true);

    }

    public void Despawn()
    {
        animator.SetTrigger("despawn");
    }
}
