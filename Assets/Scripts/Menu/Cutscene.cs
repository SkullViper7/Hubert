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
    [SerializeField] AnimationClip _hideDoc;
    [SerializeField] AnimationClip _showDoc;

    public void CallCutscene()
    {
        _UIAnimator.Play(_hideDoc.name);

        Invoke(nameof(PlayCutscene), 1f);
        Invoke(nameof(EndCutscene), _hubertClip.length);
    }

    void PlayCutscene()
    {
        _player.localPosition = Vector3.zero;
        _vfx.SetActive(true);
        _hubertAnimator.Play(_hubertClip.name);
        _trapAnimator.Play(_trapClip.name);
    }

    void EndCutscene()
    {
        _UIAnimator.Play(_showDoc.name);
    }
}
