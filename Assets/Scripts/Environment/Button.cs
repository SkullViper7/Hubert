using UnityEngine;

public class Button : MonoBehaviour
{
    public bool CanPress;
    [SerializeField] GameObject _hint;
    MeshRenderer _meshRenderer;
    [SerializeField] Material _pressedMaterial;

    [SerializeField] Animator _door;
    [SerializeField] AnimationClip _openDoorClip;

    [Header("Audio")]
    [SerializeField] AudioClip _buttonPress;

    InputManager _inputManager;
    AudioSource _audioSource;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
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
            _meshRenderer.material = _pressedMaterial;
            _door.Play(_openDoorClip.name);
            _audioSource.PlayOneShot(_buttonPress);
        }
    }
}
