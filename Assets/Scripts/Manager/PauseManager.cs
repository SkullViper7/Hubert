using UnityEngine;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    [HideInInspector] public bool IsPaused;

    [Header("UI")]
    [SerializeField] GameObject _pauseMenuUI;

    [Header("Volume")]
    [SerializeField] Animator _volumeAnimator;
    [SerializeField] AnimationClip _blur;
    [SerializeField] AnimationClip _unblur;

    InputManager _inputManager;

    private void Start()
    {
        _inputManager = GameObject.Find("Player").GetComponent<InputManager>();

        _inputManager.OnPause += PauseInput;
    }

    void PauseInput()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        _pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _volumeAnimator.Play(_blur.name);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        _pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _volumeAnimator.Play(_unblur.name);
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}
