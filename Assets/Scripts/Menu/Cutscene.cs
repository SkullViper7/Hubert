using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] Animator _hubertAnimator;
    [SerializeField] Animator _trapAnimator;

    [Space]
    [SerializeField] AnimationClip _hubertClip;
    [SerializeField] AnimationClip _trapClip;

    [Space]
    [SerializeField] GameObject _vfx;

    [Space]
    [SerializeField] Transform _player;

    [Space]
    [SerializeField] Animator _UIAnimator;
    [SerializeField] AnimationClip _UIAnimationClip;

    public void CallCutscene()
    {
        _UIAnimator.Play(_UIAnimationClip.name);

        Invoke(nameof(PlayCutscene), 1f);
    }

    void PlayCutscene()
    {
        _player.localPosition = Vector3.zero;
        _vfx.SetActive(true);
        _hubertAnimator.Play(_hubertClip.name);
        _trapAnimator.Play(_trapClip.name);
    }
}
