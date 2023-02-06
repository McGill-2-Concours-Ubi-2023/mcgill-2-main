using System;
using Unity.Mathematics;
using UnityEngine;

public class MainCharacterFreeFallState : GenericStateMachineBehaviour<MainCharacterFreeFallStateBehaviour>
{
    
}

public class MainCharacterFreeFallStateBehaviour : GenericStateMachineMonoBehaviour
{
    private readonly static int FreeFallShouldLand = Animator.StringToHash("FreeFallShouldLand");
    private float3 m_Velocity;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MainCharacterFreeFallCheckAction freeFallCheckAction = new MainCharacterFreeFallCheckAction();
        if (!freeFallCheckAction.ShouldTransition(animator.gameObject))
        {
            animator.SetTrigger(FreeFallShouldLand);
            return;
        }
    }

    private void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        m_Velocity += (float3) Physics.gravity * Time.deltaTime;
        controller.Move(m_Velocity * Time.deltaTime);
    }
}

