using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    [SerializeField]
    private bool _isCursorLocked = true;

    private void Start()
    {
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