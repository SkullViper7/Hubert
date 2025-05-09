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
    PlayStation, 
    Switch
}

public class DeviceManager : MonoBehaviour
{
    /// <summary>
    /// Event to indicate a changer in the device used.
    /// </summary>
    public event Action<DeviceType> OnDeviceTypeChanged;

    /// <summary>
    /// The last device used by the player.
    /// </summary>
    private InputDevice _lastDevice;

    /// <summary>
    /// The current device type used by the player.
    /// </summary>
    private DeviceType _currentDeviceType;

    /// <summary>
    /// Player input component of the player.
    /// </summary>
    private PlayerInput _playerInput;

    private void Awake()
    {
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
            if (device is SwitchProControllerHID)
            {
                _currentDeviceType = DeviceType.Switch;
            }
            else if (device is XInputController || device is XInputControllerWindows)
            {
                _currentDeviceType = DeviceType.Xbox;
            }
            else if (device is DualSenseGamepadHID || device is DualShock3GamepadHID || device is DualShock4GamepadHID || device is DualShockGamepad)
            {
                _currentDeviceType = DeviceType.PlayStation;
            }
            else
            {
                _currentDeviceType = DeviceType.GamepadGeneric;
            }
        }
        else if (device is Keyboard || device is Mouse)
        {
            _currentDeviceType = DeviceType.KeyboardMouse;
        }
        else
        {
            _currentDeviceType = DeviceType.Unknown;
        }

        // Update
        OnDeviceTypeChanged?.Invoke(_currentDeviceType);
    }
}
