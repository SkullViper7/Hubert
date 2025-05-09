using UnityEngine;

public class InputHelperInGame : MonoBehaviour
{
    /// <summary>
    /// All pictos of the input depending of the controller.
    /// </summary>
    [SerializeField]
    private InputUIData _inputPicto;

    /// <summary>
    /// Component which manages devices.
    /// </summary>
    private DeviceManager _deviceManager;

    /// <summary>
    /// A value indicating if the player is in.
    /// </summary>
    private bool _isActive;

    private void Awake()
    {
        _deviceManager = DeviceManager.Instance;
        if (_deviceManager != null )
        {
            _deviceManager.OnDeviceTypeChanged += UpdateUI;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_deviceManager == null)
        {
            _deviceManager = DeviceManager.Instance;
            if (_deviceManager != null)
            {
                _deviceManager.OnDeviceTypeChanged += UpdateUI;
            }
        }

        if (other.CompareTag("Player"))
        {
            _isActive = true;
            Debug.Log(_deviceManager.CurrentDeviceType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isActive = false;
        }
    }

    private void UpdateUI(DeviceType _deviceType)
    {
        if (_isActive)
        {
            Debug.Log(_deviceType);
        }
    }
}
