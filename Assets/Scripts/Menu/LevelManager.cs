using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool IsCustsceneFinished;

    [SerializeField] Animator _looneyTunesAnimator;
    [SerializeField] AnimationClip _roundClose;

    public void StartGame()
    {
        StartCoroutine(AsyncLoad());
    }

    public void SetCustsceneFinished()
    {
        IsCustsceneFinished = true;
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneBuildIndex: 1);
        async.allowSceneActivation = false;

        if (FirstLaunchManager.Instance.IsFirstLaunch)
        {
            yield return new WaitUntil(() => IsCustsceneFinished);
        }

        _looneyTunesAnimator.Play(_roundClose.name);

        yield return new WaitUntil(() =>
                    _looneyTunesAnimator.GetCurrentAnimatorStateInfo(0).IsName(_roundClose.name) &&
                    _looneyTunesAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        FirstLaunchManager.Instance.LaunchGame();

        async.allowSceneActivation = true;
    }
}
