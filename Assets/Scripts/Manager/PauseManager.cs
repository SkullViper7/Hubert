using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public PauseManager Instance { get; private set; }

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

    GameObject _pauseMenuUI;
    InputManager _inputManager;

    private void Start()
    {
        _pauseMenuUI = GameObject.Find("PauseMenu");
        _inputManager = GameObject.Find("Player").GetComponent<InputManager>();

        _inputManager.OnPause += PauseGame;
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        _pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        _pauseMenuUI.SetActive(false);
    }
}
