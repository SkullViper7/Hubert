using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.LowLevel;

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
    [SerializeField]
    private DeviceType _currentDeviceType;

    private PlayerInput _playerInput;

    private InputDevice _lastUsedDevice;

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

    private void OnControlsChanged(PlayerInput obj)
    {
        Debug.Log(Gamepad.all);

        if (obj.devices.Count > 0)
        {
            _lastUsedDevice = obj.devices[0]; // Premier device actif
            Debug.Log("Last used device: " + _lastUsedDevice.displayName + " (" + _lastUsedDevice.layout + ")");
        }
    }

    /// <summary>
    /// Called when an input is triggered to update the device used.
    /// </summary>
    /// <param name="eventPtr"></param>
    /// <param name="device"> Device used for the input. </param>
    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Prevent empty sate events
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

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
