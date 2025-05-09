using UnityEngine;

public class FragmentVanishing : StateMachineBehaviour
{
    private float _timer;
    private float _animationDuration;
    private bool _destroyed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _destroyed = false;
        _timer = 0f;
        _animationDuration = stateInfo.length;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer += Time.deltaTime;

        if (!_destroyed && _timer >= _animationDuration)
        {
            _destroyed = true;
            Destroy(animator.gameObject);
        }
    }
}
