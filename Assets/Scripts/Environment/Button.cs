using System;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public class Button : MonoBehaviour
{
    [HideInInspector] public bool CanPress;
    [SerializeField] GameObject _hint;
    [SerializeField] Animator _door;
    [SerializeField] AnimationClip _openDoorClip;

    [Header("Cables")]
    [SerializeField] List<MeshRenderer> _cablesRenderer;
    [SerializeField] Material _onMaterial;

    [Header("UI")]
    [SerializeField] InputUIData _interact;
    [SerializeField] SpriteRenderer _icon;

    public static event Action OnPressedButton;

    InputManager _inputManager;

    private void Awake()
    {
        GameManager.Instance.OnPlayerInstanciated += (PlayerStateManager player) => DeviceManager.Instance.OnDeviceTypeChanged += SwitchIcon;
    }

    void Start()
    {
        SwitchIcon(DeviceManager.Instance.CurrentDeviceType);
    }

    void SwitchIcon(DeviceType deviceType)
    {
        switch (deviceType)
        {
            case DeviceType.KeyboardMouse:
                _icon.sprite = _interact.KeyboardMouseSprite;
                break;
            
            case DeviceType.Xbox:
                _icon.sprite = _interact.XboxSprite;
                break;
            
            case DeviceType.Dualshock3:
                _icon.sprite = _interact.Dualshock3Sprite;
                break;
            
            case DeviceType.Dualshock4:
                _icon.sprite = _interact.Dualshock4Sprite;
                break;
            
            case DeviceType.Dualsense:
                _icon.sprite = _interact.Dualsense;
                break;
            
            case DeviceType.Switch:
                _icon.sprite = _interact.SwitchSprite;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CanPress = true;
            _hint.SetActive(true);
            _inputManager = other.gameObject.GetComponent<InputManager>();
            _inputManager.OnInteract += PressButton;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CanPress = false;
            _hint.SetActive(false);
        }
    }

    public void PressButton()
    {
        if (CanPress)
        {
            _door.Play(_openDoorClip.name);
            OnPressedButton?.Invoke();
            foreach (var cable in _cablesRenderer)
            {
                cable.material = _onMaterial;
            }
        }
    }
}
