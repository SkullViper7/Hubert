using UnityEngine;

public class ProgressionReseter : MonoBehaviour
{
    public void ResetProgression()
    {
        PlayerPrefs.SetInt("LastCheckpoint", 1);
        PlayerPrefs.SetInt("IsFirstLaunch", 0);
    }
}
