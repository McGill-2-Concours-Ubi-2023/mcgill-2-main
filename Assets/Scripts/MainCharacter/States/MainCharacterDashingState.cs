using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class MainCharacterDashingState : GenericStateMachineBehaviour<MainCharacterDashingStateBehaviour>
{
}

public class MainCharacterDashingStateBehaviour : GenericStateMachineMonoBehaviour
{
    private readonly static int MovementToDash = Animator.StringToHash("MovementToDash");
    private Rigidbody m_Rigidbody;
    private float3 m_Forward;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(MovementToDash);
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = transform.forward * GetComponent<MainCharacterController>().DashSpeed;
        m_Forward = transform.forward;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // expected velocity
        float3 expectedVelocity = m_Forward * GetComponent<MainCharacterController>().DashSpeed;
        float3 force = m_Rigidbody.mass * expectedVelocity * Time.fixedDeltaTime;
        m_Rigidbody.AddForce(force, ForceMode.Impulse);
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float3 expectedVelocity = float3(0, 0, 0);
        float3 force = (expectedVelocity - (float3) m_Rigidbody.velocity) * m_Rigidbody.mass / Time.fixedDeltaTime * 0.6f;
        m_Rigidbody.AddForce(force, ForceMode.Force);
        m_Rigidbody.angularVelocity = Vector3.zero;
    }
}

