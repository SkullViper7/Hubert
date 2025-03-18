using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    [SerializeField]
    private bool _isCursorLocked;

    private void Start()
    {
        Cursor.visible = !_isCursorLocked;
        Cursor.lockState = _isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}