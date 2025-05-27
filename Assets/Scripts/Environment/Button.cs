using UnityEngine;

public class Button : MonoBehaviour
{
    [HideInInspector] public bool CanPress;
    [SerializeField] GameObject _hint;
    [SerializeField] Animator _door;
    [SerializeField] AnimationClip _openDoorClip;

    [Header("Audio")]
    [SerializeField] AudioClip _buttonPress;

    InputManager _inputManager;
    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
            _audioSource.PlayOneShot(_buttonPress);
        }
    }
}
