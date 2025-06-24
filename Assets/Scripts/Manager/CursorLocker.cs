using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorLocker : MonoBehaviour
{
    [SerializeField]
    private bool _isCursorLocked = true;

    private bool _isInMenu;

    private void Start()
    {
        _isInMenu = SceneManager.GetActiveScene().buildIndex == 0;
        if (_isInMenu)
        {
            return;
        }
        Cursor.visible = !_isCursorLocked;
        Cursor.lockState = _isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (FindFirstObjectByType<CursorLocker>() == null)
        {
            GameObject go = new("CursorLocker");
            go.AddComponent<CursorLocker>();
            DontDestroyOnLoad(go);
        }
    }
}