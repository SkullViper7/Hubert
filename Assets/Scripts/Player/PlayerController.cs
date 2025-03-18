using Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Speed of the player when he walks normally.
    /// </summary>
    [Header("Movements"), SerializeField]
    private float _walkSpeed;

    /// <summary>
    /// Speed of the player when he crawls.
    /// </summary>
    [SerializeField]
    private float _crawlSpeed;

    /// <summary>
    /// A value to add smoothness to the movement.
    /// </summary>
    [SerializeField]
    private float _moveSmoothness;

    /// <summary>
    /// Speed of the rotation of the player.
    /// </summary>
    [SerializeField]
    private float _rotationSpeed;

    /// <summary>
    /// Target velocity of the velocity.
    /// </summary>
    [SerializeField]
    private Vector3 _targetVelocity;

    /// <summary>
    /// Current velocity of the player.
    /// </summary>
    private Vector3 _currentVelocity;

    /// <summary>
    /// A value indicating if the player is crawling or not.
    /// </summary>
    private bool _isCrawling;

    /// <summary>
    /// Camera of the player.
    /// </summary>
    [Space, Header("Camera"), SerializeField]
    private CinemachineFreeLook _camera;

    /// <summary>
    /// Sensitivity of the mouse to look around the player.
    /// </summary>
    [SerializeField]
    private float _mouseSensitivityX = 0.2f;

    /// <summary>
    /// Sensitivity of the mouse to zoom on the player.
    /// </summary>
    [SerializeField]
    private float _mouseSensitivityY = 0.2f;

    /// <summary>
    /// Sensitivity of the gamepad to look around the player.
    /// </summary>
    [SerializeField]
    private float _gamepadSensitivityX = 100f;

    /// <summary>
    /// Sensitivity of the gamepad to zoom on the player.
    /// </summary>
    [SerializeField]
    private float _gamepadSensitivityY = 100f;

    /// <summary>
    /// Body of the player.
    /// </summary>
    [SerializeField]
    private GameObject _playerBody;

    /// <summary>
    /// Controller component of the player.
    /// </summary>
    private CharacterController _characterController;

    /// <summary>
    /// Manager of the player inputs.
    /// </summary>
    private InputManager _inputManager;

    private float _gravityForce = 50f;
    private Vector3 _gravityVelocity;

    [SerializeField]
    private float _targetYAxis; // Valeur cible de l'axe Y
    [SerializeField]
    private float _currentYAxis;
    [SerializeField] private float _zoomSmoothness = 5f; // Vitesse de transition

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        _inputManager.OnMove += CalculateVelocity;
        _inputManager.OnCrowlStarted += () => _isCrawling = true;
        _inputManager.OnCrowlCancelled += () => _isCrawling = false;
        _inputManager.OnLookWithMouse += LookWithMouse;
        _inputManager.OnLookWithGamepad += LookWithGamepad;
        _inputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _inputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;
    }

    private void CalculateVelocity(Vector2 direction)
    {
        if (_camera == null) return;

        // Calculer la direction de la caméra par rapport au joueur
        Vector3 cameraDirection = (transform.position - _camera.transform.position).normalized;

        // Annuler l'axe vertical pour éviter que le joueur se déplace vers le haut/bas
        cameraDirection.y = 0;
        cameraDirection.Normalize();

        // Calculer un axe "droite" perpendiculaire à cette direction
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraDirection).normalized;

        // Appliquer la direction de mouvement en fonction de la caméra
        _targetVelocity = (cameraDirection * direction.y + cameraRight * direction.x) * (_isCrawling ? _crawlSpeed : _walkSpeed);
    }

    /// <summary>
    /// Called to move the player and rotate him.
    /// </summary>
    private void Move()
    {
        // Calculate velocity whith acceleration and deceleration
        _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, _moveSmoothness * Time.deltaTime);

        // Avoid residual speed that would prevent a complete stop
        if (_targetVelocity.sqrMagnitude == 0 && _currentVelocity.sqrMagnitude < 0.01f)
        {
            _currentVelocity = Vector3.zero;
        }

        // Gestion de la gravité
        if (_characterController.isGrounded)
        {
            _gravityVelocity.y = -_gravityForce * Time.deltaTime; // Empêche le personnage de "flotter" en le collant au sol
        }
        else
        {
            _gravityVelocity.y -= _gravityForce * Time.deltaTime; // Accumule la gravité
        }

        // Application du mouvement + gravité
        _characterController.Move((_currentVelocity + _gravityVelocity) * Time.deltaTime);

        // Appliquer la rotation uniquement si on se déplace
        if (_currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_currentVelocity.x, 0, _currentVelocity.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public void LookWithMouse(Vector2 direction)
    {
        if (_camera == null) return;

        // Rotation horizontale
        _camera.m_XAxis.Value += direction.x * _mouseSensitivityX;
    }

    public void LookWithGamepad(Vector2 direction)
    {
        if (_camera == null) return;

        // Rotation horizontale
        _camera.m_XAxis.Value += direction.x * _gamepadSensitivityX * Time.deltaTime;
    }

    private void CalculateZoomValueWithMouse(float value)
    {
        if (_camera == null) return;

        // Définir une cible au lieu d'appliquer directement la valeur
        _targetYAxis = Mathf.Clamp01(_targetYAxis + value * _mouseSensitivityY);
    }


    private void CalculateZoomValueWithGamepad(float value)
    {
        if (_camera == null) return;

        // Définir une cible au lieu d'appliquer directement la valeur
        _targetYAxis = Mathf.Clamp01(_targetYAxis + value * _gamepadSensitivityY);
    }

    public void Zoom()
    {
        if (_camera == null) return;

        // Lerp pour une transition fluide vers la cible
        _camera.m_YAxis.Value = Mathf.Lerp(_camera.m_YAxis.Value, _targetYAxis, _zoomSmoothness * Time.deltaTime);
        _currentYAxis = _camera.m_YAxis.Value;
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }
}
