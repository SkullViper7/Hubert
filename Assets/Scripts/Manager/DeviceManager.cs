using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

/// <summary>
/// An enum with all device types.
/// </summary>
public enum DeviceType
{
    Unknown,
    KeyboardMouse,
    GamepadGeneric,
    Xbox, 
    Dualshock3,
    Dualshock4,
    Dualsense,
    Switch
}

public class DeviceManager : MonoBehaviour
{
    // Singleton
    private static DeviceManager _instance = null;
    public static DeviceManager Instance => _instance;

    /// <summary>
    /// Event to indicate a changer in the device used.
    /// </summary>
    public event Action<DeviceType> OnDeviceTypeChanged;

    /// <summary>
    /// The current device type used by the player.
    /// </summary>
    [field: SerializeField]
    public DeviceType CurrentDeviceType { get; private set; }

    /// <summary>
    /// The last device used by the player.
    /// </summary>
    private InputDevice _lastDevice;

    /// <summary>
    /// Player input component of the player.
    /// </summary>
    private PlayerInput _playerInput;

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

        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        _playerInput.onControlsChanged -= OnControlsChanged;
    }

    /// <summary>
    /// Called when controls has changed to update the device used.
    /// </summary>
    /// <param name="playerInput"> Player input component of the player. </param>
    private void OnControlsChanged(PlayerInput playerInput)
    {
        InputDevice device = null;

        if (playerInput.devices.Count > 0)
        {
            device = playerInput.devices[0];
        }

        if (device == null)
            return;

        if (device == _lastDevice)
            return;

        _lastDevice = device;

        // Convert device into a device type
        if (device is Gamepad)
        {
            if (device is XInputController)
            {
                CurrentDeviceType = DeviceType.Xbox;
            }
            else if (device is DualShock3GamepadHID)
            {
                CurrentDeviceType = DeviceType.Dualshock3;
            }
            else if (device is DualShock4GamepadHID)
            {
                CurrentDeviceType = DeviceType.Dualshock4;
            }
            else if (device is DualSenseGamepadHID)
            {
                CurrentDeviceType = DeviceType.Dualsense;
            }
            if (device is SwitchProControllerHID)
            {
                CurrentDeviceType = DeviceType.Switch;
            }
            else
            {
                CurrentDeviceType = DeviceType.GamepadGeneric;
            }
        }
        else if (device is Keyboard || device is Mouse)
        {
            CurrentDeviceType = DeviceType.KeyboardMouse;
        }
        else
        {
            CurrentDeviceType = DeviceType.Unknown;
        }

        // Update
        OnDeviceTypeChanged?.Invoke(CurrentDeviceType);
    }
}
