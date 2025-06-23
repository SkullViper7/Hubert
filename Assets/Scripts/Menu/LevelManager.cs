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

    IEnumerator AsyncLoad()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneBuildIndex: 1);

        yield return new WaitUntil(() => IsCustsceneFinished);

        async.allowSceneActivation = true;
    }
}
