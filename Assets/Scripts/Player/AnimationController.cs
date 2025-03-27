using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartCrawl()
    {
        _animator.SetBool("IsCrawling", true);
    }

    public void StopCrawl()
    {
        _animator.SetBool("IsCrawling", false);
    }

    public void StartStick()
    {
        _animator.SetBool("IsSticked", true);
    }

    public void StopStick()
    {
        _animator.SetBool("IsSticked", false);
    }
}
