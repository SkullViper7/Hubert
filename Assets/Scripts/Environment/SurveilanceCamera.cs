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

    void Start()
    {
        _audioSource = GetComponentInParent<AudioSource>();
        _enemyVision = GetComponentInChildren<EnemyVision>();

        _enemyVision.OnPlayerSeenPos += FindPlayer;
        _enemyVision.OnPlayerLost += StartRotation;

        _startYRotation = transform.eulerAngles.y;
        StartCoroutine(Rotate());
    }

    void FindPlayer(Vector3 playerTransform)
    {
        if (!CanFollowPlayer)
        {
            _audioSource.PlayOneShot(_alert);
        }

        PlayerTransform = playerTransform;
        CanFollowPlayer = true;

        transform.LookAt(playerTransform);
    }

    void StartRotation()
    {
        CanFollowPlayer = false;
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        float currentAngle = 0f;
        int direction = 1;

        while (!CanFollowPlayer)
        {
            while ((direction == 1 && currentAngle < _maxRightAngle) ||
           (direction == -1 && currentAngle > _maxLeftAngle))
            {
                if (!_audioSource.isPlaying)
                    _audioSource.PlayOneShot(_moving);

                float angleThisFrame = _rotationSpeed * Time.deltaTime * direction;
                currentAngle += angleThisFrame;

                float yRotation = _startYRotation + currentAngle;
                transform.rotation = Quaternion.Euler(_verticalRotation, yRotation, 0);

                yield return null;
            }

            currentAngle = Mathf.Clamp(currentAngle, _maxLeftAngle, _maxRightAngle);

            direction *= -1;

            _audioSource.Stop();

            yield return new WaitForSeconds(_stoppingTime);
        }
    }
}
