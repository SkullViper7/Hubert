using UnityEngine;

public class Button : MonoBehaviour
{
    public bool CanPress;
    [SerializeField] GameObject _hint;
    MeshRenderer _meshRenderer;
    [SerializeField] Material _pressedMaterial;

    [SerializeField] Animator _door;
    [SerializeField] AnimationClip _openDoorClip;

    InputManager _inputManager;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
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
        }
    }
}
