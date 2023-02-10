using UnityEngine;
using UnityEngine.Animations;


public class GenericStateMachineBehaviour<TBehaviour> : StateMachineBehaviour
    where TBehaviour : GenericStateMachineMonoBehaviour, new()
{
    private TBehaviour associatedBehaviour;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        associatedBehaviour = animator.gameObject.AddComponent<TBehaviour>();
        associatedBehaviour.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        associatedBehaviour.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        associatedBehaviour.OnStateExit(animator, stateInfo, layerIndex);
        Destroy(associatedBehaviour);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        associatedBehaviour.OnStateMove(animator, stateInfo, layerIndex);
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        associatedBehaviour.OnStateIK(animator, stateInfo, layerIndex);
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        associatedBehaviour.OnStateMachineEnter(animator, stateMachinePathHash);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        associatedBehaviour.OnStateMachineExit(animator, stateMachinePathHash);
    }

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateEnter(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateUpdate(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateExit(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateMove(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateMove(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateIK(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateIK(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateMachineEnter(
        Animator animator,
        int stateMachinePathHash,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateMachineEnter(animator, stateMachinePathHash, controller);
    }

    public override void OnStateMachineExit(
        Animator animator,
        int stateMachinePathHash,
        AnimatorControllerPlayable controller)
    {
        associatedBehaviour.OnStateMachineExit(animator, stateMachinePathHash, controller);
    }
}
