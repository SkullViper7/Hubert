using System.Collections;
using UnityEngine;

public class SurveilanceCamera : MonoBehaviour
{
    AudioSource _audioSource;
    EnemyVision _enemyVision;

    [Header("Settings")]
    [SerializeField] float _rotationSpeed = 1f;
    [SerializeField] float _stoppingTime = 1f;
    [SerializeField] float _maxLeftAngle = -45f;
    [SerializeField] float _maxRightAngle = 45f;
    [SerializeField] float _verticalRotation = 25f;
    float _startYRotation;

    [Header("Audio")]
    [SerializeField] AudioClip _moving;
    [SerializeField] AudioClip _alert;

    [HideInInspector] public bool CanFollowPlayer;
    [HideInInspector] public Vector3 PlayerTransform;

    Coroutine _rotationCoroutine;
    float _currentAngle = 0f;


    void Start()
    {
        _audioSource = GetComponentInParent<AudioSource>();
        _enemyVision = GetComponentInChildren<EnemyVision>();

        _enemyVision.OnPlayerSeenPos += FindPlayer;
        _enemyVision.OnPlayerLost += StartRotation;

        _startYRotation = transform.eulerAngles.y;
        _rotationCoroutine = StartCoroutine(Rotate());
    }

    void FindPlayer(Vector3 playerTransform)
    {
        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        if (!CanFollowPlayer)
        {
            _audioSource.PlayOneShot(_alert);
        }

        PlayerTransform = playerTransform;
        CanFollowPlayer = true;

        StartCoroutine(SmoothLookAt(playerTransform));
    }

    IEnumerator SmoothLookAt(Vector3 targetPosition)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Quaternion initialRotation = transform.rotation;

        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0f;
        if (directionToTarget == Vector3.zero)
            yield break;

        Quaternion fullLookRotation = Quaternion.LookRotation(directionToTarget);

        float targetY = fullLookRotation.eulerAngles.y;
        float relativeY = Mathf.DeltaAngle(_startYRotation, targetY);
        float clampedRelativeY = Mathf.Clamp(relativeY, _maxLeftAngle, _maxRightAngle);
        float finalY = _startYRotation + clampedRelativeY;

        Quaternion targetRotation = Quaternion.Euler(_verticalRotation, finalY, 0f);

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        _currentAngle = clampedRelativeY;
    }

    void StartRotation()
    {
        CanFollowPlayer = false;
        _rotationCoroutine = StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        int direction = 1;

        while (!CanFollowPlayer)
        {
            while ((direction == 1 && _currentAngle < _maxRightAngle) ||
           (direction == -1 && _currentAngle > _maxLeftAngle))
            {
                if (!_audioSource.isPlaying)
                    _audioSource.PlayOneShot(_moving);

                float angleThisFrame = _rotationSpeed * Time.deltaTime * direction;
                _currentAngle += angleThisFrame;

                float yRotation = _startYRotation + _currentAngle;
                transform.rotation = Quaternion.Euler(_verticalRotation, yRotation, 0);

                yield return null;
            }

            _currentAngle = Mathf.Clamp(_currentAngle, _maxLeftAngle, _maxRightAngle);

            direction *= -1;

            _audioSource.Stop();

            yield return new WaitForSeconds(_stoppingTime);
        }
    }
}
