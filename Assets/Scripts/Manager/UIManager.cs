using UnityEngine;

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
        GameManager.Instance.OnPlayerDead += () => _minimap.SetActive(false);
    }
}
