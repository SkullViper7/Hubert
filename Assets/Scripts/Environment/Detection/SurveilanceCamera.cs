using System.Collections;
using UnityEngine;

public class SurveilanceCamera : DetectionObject
{
    /// <summary>
    /// The left limit angle of the camera.
    /// </summary>
    [SerializeField, Header("Patrol")]
    private float _leftLimit = -45f;

    /// <summary>
    /// The right limit angle of the camera.
    /// </summary>
    [SerializeField]
    private float _rightLimit = 45f;

    /// <summary>
    /// Current angle of the patrol.
    /// </summary>
    private float _patrolAngle;

    /// <summary>
    /// Speed of the rotation.
    /// </summary>
    [SerializeField]
    private float _rotationSpeed = 30f;

    /// <summary>
    /// Time of the pause before changing direction in the patrol.
    /// </summary>
    [SerializeField]
    private float _pauseTime = 1f;

    /// <summary>
    /// A value indicating if the patrol is in pause.
    /// </summary>
    private bool _isPaused = false;
    
    /// <summary>
    /// Direction of the patrol.
    /// </summary>
    [SerializeField]
    private int _direction = 1;

    /// <summary>
    /// A value indicating if the camera is recalibrating to the patrol.
    /// </summary>
    private bool _isRecalibrating = false;

    /// <summary>
    /// The vision component.
    /// </summary>
    [SerializeField, Space, Header("Vision")]
    private EnemyVision _enemyVision;

    /// <summary>
    /// The position of the target.
    /// </summary>
    private Vector3 _lookTarget;

    /// <summary>
    /// A value indicating if the camera has a target.
    /// </summary>
    private bool _hasTarget = false;

    private void Start()
    {
        _enemyVision.OnPlayerSeen += SetLookTarget;
        _patrolAngle = NormalizeAngle(transform.localEulerAngles.y);
        StartCoroutine(PatrolRoutine());
    }

    private void Update()
    {
        if (_hasTarget)
        {
            LookAtTarget();
        }
        else if (_isRecalibrating)
        {
            float currentY = NormalizeAngle(transform.localEulerAngles.y);
            float newY = Mathf.MoveTowardsAngle(currentY, _patrolAngle, _rotationSpeed * Time.deltaTime);

            Vector3 newEuler = transform.localEulerAngles;
            newEuler.y = newY;
            transform.localEulerAngles = newEuler;

            // Stop recalibration once aligned
            if (Mathf.Approximately(NormalizeAngle(newY), _patrolAngle))
            {
                _isRecalibrating = false;
            }
        }
    }

    /// <summary>
    /// called to do a left right patrol.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // Continuous calculation of the theoretical angle, even if it is not used
            if (!_isPaused)
            {
                float angle = _rotationSpeed * Time.deltaTime * _direction;
                _patrolAngle += angle;
                _patrolAngle = NormalizeAngle(_patrolAngle);

                // Reverse direction if limits are reached
                if (_direction == 1 && _patrolAngle >= _rightLimit)
                {
                    _direction = -1;
                    _isPaused = true;
                    yield return new WaitForSeconds(_pauseTime);
                    _isPaused = false;
                }
                else if (_direction == -1 && _patrolAngle <= _leftLimit)
                {
                    _direction = 1;
                    _isPaused = true;
                    yield return new WaitForSeconds(_pauseTime);
                    _isPaused = false;
                }

                // Apply actual angle if patrol active
                if (!_hasTarget && !_isRecalibrating)
                {
                    Vector3 newEuler = transform.localEulerAngles;
                    newEuler.y = Mathf.MoveTowardsAngle(NormalizeAngle(newEuler.y), _patrolAngle, _rotationSpeed * Time.deltaTime);
                    transform.localEulerAngles = newEuler;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Called to look into the direction of a target when there is one.
    /// </summary>
    private void LookAtTarget()
    {
        Vector3 worldDirection = _lookTarget - transform.position;
        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.01f)
            return;

        // Calculate local direction
        Vector3 localDirection = transform.parent
            ? transform.parent.InverseTransformDirection(worldDirection)
            : worldDirection;

        float targetAngleY = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        targetAngleY = NormalizeAngle(targetAngleY);

        // Clamp within patrol limits
        float clampedAngleY = Mathf.Clamp(targetAngleY, _leftLimit, _rightLimit);

        // Current angle
        float currentAngleY = NormalizeAngle(transform.localEulerAngles.y);

        // Interpolation to clamped angle
        float newY = Mathf.MoveTowardsAngle(currentAngleY, clampedAngleY, _rotationSpeed * Time.deltaTime);

        Vector3 newLocalEuler = transform.localEulerAngles;
        newLocalEuler.y = newY;
        transform.localEulerAngles = newLocalEuler;
    }

    /// <summary>
    /// Called to set a target position.
    /// </summary>
    /// <param name="targetPosition"> The position of the target. </param>
    /// <param name="context"> The context of the vision. </param>
    public void SetLookTarget(Vector3 targetPosition, PlayerSeenContext context)
    {
        if (context == PlayerSeenContext.FirstTime || context == PlayerSeenContext.Continue)
        {
            targetPosition.y = transform.position.y;
            _lookTarget = targetPosition;
            _hasTarget = true;
            _isRecalibrating = false; // We follow a target
        }
        else if (context == PlayerSeenContext.LastTime)
        {
            _hasTarget = false;
            _isRecalibrating = true; // We have to recalibrate
        }
    }

    /// <summary>
    /// Called to normalize an angle.
    /// </summary>
    /// <param name="angle"> The angle to normalize. </param>
    /// <returns></returns>
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
