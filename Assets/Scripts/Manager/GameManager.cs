using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    /// <summary>
    /// A value to indicate if the game is running or not.
    /// </summary>
    public bool IsGameRunning { get; private set; }

    /// <summary>
    /// A reference to the pause menu UI.
    /// </summary>
    [Header("UI")]
    [SerializeField] GameObject _pauseMenuUI;

    /// <summary>
    /// A reference to the volume animator.
    /// </summary>
    [Header("Volume")]
    [SerializeField] Animator _volumeAnimator;
    [SerializeField] AnimationClip _blur;
    [SerializeField] AnimationClip _unblur;

    /// <summary>
    /// A reference to the input manager.
    /// </summary>
    InputManager _inputManager;

    /// <summary>
    /// A reference to the player.
    /// </summary>
    public PlayerStateManager Player { get; private set; }

    /// <summary>
    /// An event to indicate that the player is instanciated in the scene.
    /// </summary>
    public event Action<PlayerStateManager> OnPlayerInstanciated;

    /// <summary>
    /// An event to indicate that the player is dead.
    /// </summary>
    public event Action OnPlayerDead;

    /// <summary>
    /// The player object prefab.
    /// </summary>
    [SerializeField]
    private GameObject _player;

    /// <summary>
    /// An list of checkpoints in the level.
    /// </summary>
    [SerializeField]
    private List<Checkpoint> _checkpoints;

    /// <summary>
    /// A dictionnary which stocks checkpoints and their order.
    /// </summary>
    private readonly Dictionary<int, Checkpoint> _checkpointsOrder = new();

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        SetUpCheckpoints();

        // Spawn the player at the last saved checkpoint
        SpawnPlayer(GetLastSavedCheckpoint());

        IsGameRunning = true;

        // Get the input manager
        _inputManager = _player.GetComponent<InputManager>();

        // Subscribe to the pause input
        _inputManager.OnPause += PauseInput;
    }

    /// <summary>
    /// Called to pause or resume the game.
    /// </summary>
    void PauseInput()
    {
        if (!IsGameRunning)
        {
            ResumeGame();
        }
        else
        {
            StopGame();
        }
    }

    /// <summary>
    /// Called to stop the game running.
    /// </summary>
    void StopGame()
    {
        // Pause the game
        IsGameRunning = false;
        Time.timeScale = 0f;

        // Show the pause menu
        _pauseMenuUI.SetActive(true);

        // Lock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Play the blur
        _volumeAnimator.Play(_blur.name);
    }

    /// <summary>
    /// Called to resume the game.
    /// </summary>
    void ResumeGame()
    {
        IsGameRunning = true;
        Time.timeScale = 1f;

        // Hide the pause menu
        _pauseMenuUI.SetActive(false);

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Play the unblur
        _volumeAnimator.Play(_unblur.name);
    }

    /// <summary>
    /// Called to disable the game manager.
    /// </summary>
    void OnDisable()
    {
        Time.timeScale = 1f;
        IsGameRunning = true;
    }

    /// <summary>
    /// Called to update the last checkpoint if it's a greater than the previous.
    /// </summary>
    /// <param name="value"> Value of the checkpoint. </param>
    public void UpdateCheckpoint(int value)
    {
        if (value > PlayerPrefs.GetInt("LastCheckpoint"))
        {
            PlayerPrefs.SetInt("LastCheckpoint", value);
        }
    }

    /// <summary>
    /// Called to set up checkpoints.
    /// </summary>
    private void SetUpCheckpoints()
    {
        if (_checkpoints.Count > 0)
        {
            for (int i = 0; i < _checkpoints.Count; i++)
            {
                if (_checkpoints[i] != null && !_checkpointsOrder.ContainsValue(_checkpoints[i]))
                {
                    _checkpointsOrder.Add(_checkpoints[i].CheckpointOrder, _checkpoints[i]);
                }
            }
        }
        else
        {
            Debug.LogError("No checkpoints in the level, consider adding some !");
        }
    }

    /// <summary>
    /// Called to spawn the player at a specified checkpoint.
    /// </summary>
    /// <param name="checkpointValue"> Value of the checkpoint where to spawn the player. </param>
    private void SpawnPlayer(int checkpointValue)
    {
        if (_checkpointsOrder.ContainsKey(checkpointValue))
        {
            GameObject newPlayer = Instantiate(_player, _checkpointsOrder[checkpointValue].RespawnPosition.position, _checkpointsOrder[checkpointValue].RespawnPosition.rotation);
            newPlayer.name = "Player";
            Player = newPlayer.GetComponent<PlayerStateManager>();
            _checkpointsOrder[checkpointValue].RoomAssociated.AddPlayer(Player);
            Player.OnDeath += () => OnPlayerDead?.Invoke();
            OnPlayerInstanciated?.Invoke(Player);
        }
    }

    /// <summary>
    /// Called to get the last saved checkpoint.
    /// </summary>
    /// <returns></returns>
    private int GetLastSavedCheckpoint()
    {
        if (!PlayerPrefs.HasKey("LastCheckpoint"))
        {
            PlayerPrefs.SetInt("LastCheckpoint", 1);
        }

        return PlayerPrefs.GetInt("LastCheckpoint");
    }
}
