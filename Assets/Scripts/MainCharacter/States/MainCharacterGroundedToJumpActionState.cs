using UnityEngine;
using UnityEngine.Animations;

public class MainCharacterGroundedToJumpActionState : GenericStateMachineBehaviour<MainCharacterGroundedToJumpActionStateBehaviour>
{
    
}

public class MainCharacterGroundedToJumpActionStateBehaviour : GenericStateMachineMonoBehaviour
{
    private float m_Percentage = 0.0f;
    private readonly static int GroundedToJumpActionPlaybackPercentage = Animator.StringToHash("GroundedToJumpActionPlaybackPercentage");
    private Animator m_Animator;
    private Vector3 m_JumpTarget;
    private Vector3 m_InitialPosition;
    private readonly static int GroundedToJumpAction = Animator.StringToHash("GroundedToJumpAction");
    private float m_TimeOnEnter;
    private const float maxTime = 0.5f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Animator = animator;
        m_JumpTarget = GetComponent<MainCharacterController>().NavActionData as Vector3? ?? Vector3.zero;
        m_InitialPosition = transform.position;
        m_TimeOnEnter = Time.time;
    }

    private void Update()
    {
        m_Animator.ResetTrigger(GroundedToJumpAction);
        m_Percentage += Time.deltaTime / maxTime;
        if (Time.time - m_TimeOnEnter >= maxTime)
        {
            m_Percentage = 1.0f;
            m_Animator.SetFloat(GroundedToJumpActionPlaybackPercentage, m_Percentage);
        }
        else
        {
            m_Animator.SetFloat(GroundedToJumpActionPlaybackPercentage, m_Percentage);
            CharacterController controller = GetComponent<CharacterController>();
            Vector3 jumpDirection = m_JumpTarget - m_InitialPosition;
            controller.Move(jumpDirection * (Time.deltaTime / maxTime));
        }
    }

    private void OnLand()
    {
        Debug.Log("OnLand");
    }
}

