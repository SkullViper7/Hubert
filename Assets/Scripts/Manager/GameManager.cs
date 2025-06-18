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
    /// The player object.
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
    }

    /// <summary>
    /// Called to stop the game running.
    /// </summary>
    public void StopGame()
    {
        IsGameRunning = false;
    }

    /// <summary>
    /// Called to resume the game.
    /// </summary>
    public void ResumeGame()
    {
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
            Instantiate(_player, _checkpointsOrder[checkpointValue].RespawnPosition.position, _checkpointsOrder[checkpointValue].RespawnPosition.rotation);
            _checkpointsOrder[checkpointValue].RoomAssociated.AddPlayer(_player.GetComponent<PlayerStateManager>());
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
