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

    private void Start()
    {
        Invoke(nameof(PlayCutscene), 1f);
    }

    public void PlayCutscene()
    {
        _player.localPosition = Vector3.zero;
        _vfx.SetActive(true);
        _hubertAnimator.Play(_hubertClip.name);
        _trapAnimator.Play(_trapClip.name);
    }
}
