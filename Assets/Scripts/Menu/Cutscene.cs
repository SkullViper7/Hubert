using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    bool _isMenuCutscene;

    public void SetCutsceneBool()
    {
        _isMenuCutscene = true;
    }

    public void CallCutscene()
    {
        _UIAnimator.Play(_hideDoc.name);

        Invoke(nameof(PlayCutscene), 1f);

        if (_isMenuCutscene)
        {
            Invoke(nameof(EndCutscene), _hubertClip.length);
        }
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
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        LooneyTunesManager.Instance.PlayCloseHoleAnim();

        yield return new WaitForSeconds(LooneyTunesManager.Instance.CloseHole.length);

        _isMenuCutscene = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
