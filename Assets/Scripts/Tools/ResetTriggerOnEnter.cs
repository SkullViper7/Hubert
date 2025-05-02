using UnityEngine;

public class ResetTriggerOnEnter : StateMachineBehaviour
{
    /// <summary>
    /// The name of the trigger to reset
    /// </summary>
    [SerializeField]
    private string _triggerToReset;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerToReset);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerToReset);
    }
}
