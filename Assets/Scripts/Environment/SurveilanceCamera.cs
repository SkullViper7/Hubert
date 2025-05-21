using UnityEngine;

public class SurveilanceCamera : MonoBehaviour
{
    AudioSource _audioSource;

    [SerializeField] AudioClip _moving;
    [SerializeField] AudioClip _alert;

    [HideInInspector] public bool CanFollowPlayer;
    [HideInInspector] public Transform PlayerTransform;

    void Start()
    {
        _audioSource = GetComponentInParent<AudioSource>();
    }

    void Update()
    {
        if (CanFollowPlayer)
        {
            FollowPlayer(PlayerTransform.transform);
        }
    }

    public void PlayMoving() => _audioSource.PlayOneShot(_moving);
    public void StopMoving() => _audioSource.Stop();
    public void PlayAlert() => _audioSource.PlayOneShot(_alert);

    void FollowPlayer(Transform playerTransform)
    {
        transform.LookAt(playerTransform);
    }

    void FindPlayer(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
        CanFollowPlayer = true;
    }
}
