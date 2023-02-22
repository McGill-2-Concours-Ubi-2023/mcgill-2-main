using UnityEngine;

public class MainCharacterDashingState : GenericStateMachineBehaviour<MainCharacterDashingStateBehaviour>
{
}

public class MainCharacterDashingStateBehaviour : GenericStateMachineMonoBehaviour
{
    private readonly static int MovementToDash = Animator.StringToHash("MovementToDash");
    private Rigidbody m_Rigidbody;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(MovementToDash);
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = transform.forward * GetComponent<MainCharacterController>().DashSpeed;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Rigidbody.velocity = transform.forward * GetComponent<MainCharacterController>().DashSpeed;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_Rigidbody.velocity = Vector3.zero;
    }
}

