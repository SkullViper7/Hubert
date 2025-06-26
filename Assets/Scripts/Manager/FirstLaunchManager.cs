using UnityEngine;

public class FirstLaunchManager : MonoBehaviour
{
    // Singleton
    private static FirstLaunchManager _instance = null;
    public static FirstLaunchManager Instance => _instance;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("IsFirstLaunch"))
        {
            PlayerPrefs.SetInt("IsFirstLaunch", 0);
        }
    }

    public void LaunchGame()
    {
        PlayerPrefs.SetInt("IsFirstLaunch", 1);
    }
}
