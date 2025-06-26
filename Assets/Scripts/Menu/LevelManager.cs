using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool IsCustsceneFinished;

    public void StartGame()
    {
        StartCoroutine(AsyncLoad());
    }

    public void SetCustsceneFinished()
    {
        IsCustsceneFinished = true;
    }

    private IEnumerator AsyncLoad()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneBuildIndex: 1);
        async.allowSceneActivation = false;

        if (PlayerPrefs.GetInt("IsFirstLaunch") == 0)
        {
            yield return new WaitUntil(() => IsCustsceneFinished);
        }

        LooneyTunesManager.Instance.PlayCloseHoleAnim();

        yield return new WaitForSeconds(LooneyTunesManager.Instance.CloseHole.length);

        FirstLaunchManager.Instance.LaunchGame();

        async.allowSceneActivation = true;
    }
}
