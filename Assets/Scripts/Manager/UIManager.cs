using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Singleton
    private static UIManager _instance = null;
    public static UIManager Instance => _instance;

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
}
