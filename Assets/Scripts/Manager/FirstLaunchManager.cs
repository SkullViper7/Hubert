using UnityEngine;

public class FirstLaunchManager : MonoBehaviour
{
    // Singleton
    private static FirstLaunchManager _instance = null;
    public static FirstLaunchManager Instance => _instance;

    /// <summary>
    /// A value to indicate if this is the first launch of the game.
    /// </summary>
    public bool IsFirstLaunch { get; private set; }

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
        if (!PlayerPrefs.HasKey("IsFirstLaunch") || PlayerPrefs.GetInt("IsFirstLaunch") == 0)
        {
            IsFirstLaunch = true;
        }
        else
        {
            IsFirstLaunch = false;
        }
    }

    public void LaunchGame()
    {
        IsFirstLaunch = false;
    }

    public void OnDisable()
    {
        PlayerPrefs.SetInt("IsFirstLaunch", IsFirstLaunch ? 0 : 1);
    }
}
