using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Singleton
    private static UIManager _instance = null;
    public static UIManager Instance => _instance;

    /// <summary>
    /// Minimap of the HUD;
    /// </summary>
    [SerializeField]
    private GameObject _minimap;

    /// <summary>
    /// Minimap animator of the HUD;
    /// </summary>
    public Animator MinimapAnimator;

    /// <summary>
    /// Pause menu of the HUD;
    /// </summary>
    [SerializeField]
    private GameObject _pauseMenu;

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
        GameManager.Instance.OnPlayerAlmostDead += () => MinimapAnimator.Play("MinimapHide");
    }
}
