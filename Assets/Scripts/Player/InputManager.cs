using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnCrawl, OnStick, OnHideStarted, OnHideCancelled, OnAimStarted, OnAimCancelled, OnHit, OnShoot;

    public event Action<float> OnZoomWithMouse, OnZoomWithGamepad;

    public event Action<Vector2> OnMove, OnLookWithMouse, OnLookWithGamepad;
    
    [SerializeField]
    private bool _isMoving, _isLookingWithGamepad, _isZoomingWithGamepad, _isSticking, _isHiding, _isAiming;

    private Vector2 _moveDirection, _lookDirection;

    private float _zoomValue, _switchTargetValue;

    private PlayerInput _playerInput;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.onActionTriggered += OnAction;
    }

    private void Update()
    {
        if (_isMoving)
        {
            OnMove?.Invoke(_moveDirection);
        }

        if (_isLookingWithGamepad)
        {
            OnLookWithGamepad?.Invoke(_lookDirection);
        }

        if (_isZoomingWithGamepad)
        {
            OnZoomWithGamepad?.Invoke(_zoomValue);
        }
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        string controlScheme = _playerInput.currentControlScheme;

        switch (context.action.name)
        {
            case "Move":
                if (context.performed)
                {
                    _moveDirection = context.ReadValue<Vector2>();
                    _isMoving = true;
                }
                else if (context.canceled || context.ReadValue<Vector2>() == Vector2.zero)
                {
                    _isMoving = false;
                    _moveDirection = Vector2.zero;
                    OnMove?.Invoke(_moveDirection);
                }
                break;

            case "Crowl":
                if (context.started)
                {
                    OnCrawl?.Invoke();
                }
                break;

            case "Look":
                if (controlScheme == "Keyboard&Mouse")
                {
                    _isLookingWithGamepad = false;

                    if (context.performed)
                    {
                        OnLookWithMouse?.Invoke(context.ReadValue<Vector2>());
                    }
                }
                else if (controlScheme == "Gamepad")
                {
                    if (context.performed)
                    {
                        _lookDirection = context.ReadValue<Vector2>();
                        _isLookingWithGamepad = true;
                    }
                    else if (context.canceled || context.ReadValue<Vector2>() == Vector2.zero)
                    {
                        _isLookingWithGamepad = false;
                        _lookDirection = Vector2.zero;
                        OnLookWithGamepad?.Invoke(_lookDirection);
                    }
                }
                break;

            case "Zoom":
                if (controlScheme == "Keyboard&Mouse")
                {
                    _isZoomingWithGamepad = false;

                    if (context.performed)
                    {
                        OnZoomWithMouse?.Invoke(context.ReadValue<Vector2>().y / 120 * -1);
                    }
                }
                else if (controlScheme == "Gamepad")
                {
                    if (context.performed)
                    {
                        _zoomValue = context.ReadValue<Vector2>().y * -1;
                        _isZoomingWithGamepad = true;
                    }
                    else if (context.canceled || context.ReadValue<Vector2>().y == 0)
                    {
                        _isZoomingWithGamepad = false;
                        _zoomValue = 0;
                        OnZoomWithGamepad?.Invoke(_zoomValue);
                    }
                }
                break;

            case "Stick":
                if (context.started)
                {
                    OnStick?.Invoke();
                }
                break;

                //case "Hide":
                //    if (context.started)
                //    {
                //        OnHide?.Invoke();
                //    }
                //    break;

                //case "Hit":
                //    if (context.started)
                //    {
                //        OnHit?.Invoke();
                //    }
                //    break;

                //case "Hit":
                //    if (context.started)
                //    {
                //        OnHit?.Invoke();
                //    }
                //    break;
        }
    }
}
