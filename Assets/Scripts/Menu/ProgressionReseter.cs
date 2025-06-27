using UnityEngine;

public class ProgressionReseter : MonoBehaviour
{
    [SerializeField] GameObject _text;

    public void ResetProgression()
    {
        PlayerPrefs.SetInt("LastCheckpoint", 1);
        PlayerPrefs.SetInt("IsFirstLaunch", 0);
        _text.SetActive(true);
        Invoke(nameof(HideText), 2f);
    }

    void HideText()
    {
        _text.SetActive(false);
    }
}
