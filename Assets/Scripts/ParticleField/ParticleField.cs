using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(ParticleSystem))]
public class ParticleField : MonoBehaviour
{
    public ParticleFieldInteractor[] interactors;
    //public GameObject interactor;
    [Range(0, 1f)]
    public float dampeningForce = 0.1f;    
    [Range(0, 2f)]
    public float callbackForce = 0.6f;
    [Range(0, 10f)]
    public float callbackDistance = 2.0f;
    public bool diverge = false;
    [Range(0f, 0.0001f)]
    public float divergence = 0.01f;
    [Range(0.1f, 10.0f)]
    public float agitation = 1f;

    private ParticleSystem m_System;
    private ParticleSystem.Particle[] m_Particles;
    private ParticleSystem.Particle[] m_Particles_init;   
    private Transform playerPosition;
    private ParticleSystem.MainModule main;
    private Vector3 particleOffset;
    //private Vector3 interactorDirection;

    // Start is called before the first frame update
    void Start()
    {
        InitializeParticleSystem();
        int numParticlesAlive = m_System.GetParticles(m_Particles);
        m_System.SetParticles(m_Particles, numParticlesAlive);
        m_System.Play();
        //Deep copy of the initial particle system data
        m_Particles_init = new ParticleSystem.Particle[m_System.main.maxParticles];
        for(int i = 0; i < numParticlesAlive; i++)
        {
            m_Particles_init[i] = m_Particles[i];
        }
    }

    // FixedUpdate is called every fixed frame rate, if the Behaviour is enabled
    // main.emitterVelocityMode uses RigidBody
    void FixedUpdate()
    {
        //Get all the particles
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        //We need to iterate through all particles 
        for (int i = 0; i < numParticlesAlive; i++) 
        {
            Vector3 particlePosition = m_Particles[i].position; // the particle position
            var allDirections = new Dictionary<float, ParticleFieldInteractor>(); // store each interactor data to later check which one is closer

            //O(n^2) with the number of particles, not cheap ! OUCH
            foreach (var interactor in interactors)
            {           
                //the vector from the interactor to the particle
                Vector3 interactorDirection = particlePosition - interactor.transform.position;
                allDirections.Add(interactorDirection.magnitude, interactor); // store the magnitude of the above vector and the corresponding interactor
            }
            
            float minDistance = Mathf.Min((new List<float>(allDirections.Keys)).ToArray()); //the distance from the closest interactor
            ParticleFieldInteractor closestInteractor;
            allDirections.TryGetValue(minDistance, out closestInteractor); // Retrieve the closest interactor to use its attribute localy
            Vector3 minDirection = particlePosition - closestInteractor.transform.position; //Vector from the closest interactor to the particle
            
            //Add force to the particle if within radius
            if (minDirection.magnitude < closestInteractor.influenceRadius)
            {
                m_Particles[i].velocity = new Vector3(minDirection.x * closestInteractor.pushForce + Random.Range(0.2f, 0.7f),
                    minDirection.y * closestInteractor.pushForce + Random.Range(0.3f, 0.8f),
                        minDirection.z * closestInteractor.pushForce + Random.Range(0.4f, 0.9f));
            }

            //If not withing radius, and no divergence, make the particle come back
            else if (minDirection.magnitude >= closestInteractor.influenceRadius + callbackDistance && !diverge)
            {
                particleOffset = m_Particles_init[i].position - m_Particles[i].position;
                if (particleOffset.magnitude > 0.1f)
                {
                    m_Particles[i].velocity = new Vector3(particleOffset.x * callbackForce,
                    particleOffset.y * callbackForce,
                        particleOffset.z * callbackForce);
                }
            }
            var velocity = m_Particles[i].velocity;

            //Depending if divergence or not, proportionally dampen the particle velocity over time
            if (!diverge)
            {
                m_Particles[i].velocity = new Vector3(velocity.x / (1 + dampeningForce),
                                velocity.y / (1 + dampeningForce),
                                    velocity.z / (1 + dampeningForce));
            }
            else
            {
                m_Particles[i].velocity = new Vector3(velocity.x / (1 + divergence),
                                velocity.y / (1 + divergence),
                                    velocity.z / (1 + divergence));
            }
                  
        }
       
        main.simulationSpeed = agitation; //change simulation speed if needed

        // Apply the particle changes to the Particle  System
        m_System.SetParticles(m_Particles, numParticlesAlive);  
    }

    void InitializeParticleSystem()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];

        //Default necessary parameters
        main = m_System.main;
        main.startLifetime = Mathf.Infinity; //particles spawn once
        main.startSpeed = 0f; // particles stay in place
        main.simulationSpace = ParticleSystemSimulationSpace.World; // needed for physics simulation
        main.simulationSpeed = 1f; // default agitation is one - can be updated at runtime
        main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody; //we use the particles velocity 
        var emission = m_System.emission;
        emission.rateOverTime = m_System.main.maxParticles + 1; //emission rate needs to be one above the max particles
    }
}
