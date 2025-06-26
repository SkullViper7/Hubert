using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region Room
    /// <summary>
    /// Light to activate when this is a general alert.
    /// </summary>
    [SerializeField]
    private GameObject _alarmeLight;

    /// <summary>
    /// The highest alerte level of the room, only one enemy in a greater alerte level cans change it.
    /// </summary>
    public AlerteLevel RoomAlerteLevel { get; private set; } = AlerteLevel.Patrol;

    /// <summary>
    /// An event to indicate that the room alerte level has changed.
    /// </summary>
    public event Action<AlerteLevel> OnRoomAlerteLevelChanged;

    /// <summary>
    /// An event to indicate that a general alerte has been triggered.
    /// </summary>
    public event Action<AlerteLevel> OnGeneralAlerte;

    /// <summary>
    /// An event ot indicate that the alerte level is almost finished.
    /// </summary>
    public event Action OnAlerteAlmostFinished;

    /// <summary>
    /// List of all enemies in the room.
    /// </summary>
    [field: SerializeField]
    public List<EnemyBrain> EnemiesInRoom { get; private set; } = new();

    /// <summary>
    /// The player in the room.
    /// </summary>
    private PlayerStateManager _playerInRoom;

    /// <summary>
    /// An event to indicate that the player is in the room.
    /// </summary>
    public event Action<Room> OnPlayerIsInTheRoom;

    /// <summary>
    /// List of all detection objects in the room.
    /// </summary>
    [SerializeField]
    private List<DetectionObject> _detectionObjects = new();

    /// <summary>
    /// A value indicating if there is already a general alerte.
    /// </summary>
    private bool _isThereAlreadyGeneralAlerte;

    /// <summary>
    /// A dictionary which stocks the number of enemies in a certain alerte level.
    /// </summary>
    private Dictionary<AlerteLevel, int> _enemiesAlerteLevels = new() { {AlerteLevel.Patrol, 0}, { AlerteLevel.Research, 0 }, { AlerteLevel.Alerte, 0 } };

    /// <summary>
    /// A locker to avoid that many instances can try to update alerte levels at the same time.
    /// </summary>
    private readonly object s_addAlerteLevelLocker = new(), s_removeAlerteLevelLocker = new();
    #endregion

    #region Research
    /// <summary>
    /// Events to tell to all enemies when research is ended.
    /// </summary>
    public event Action OnResearchEnded;

    /// <summary>
    /// Elapsed time since the start of the research state.
    /// </summary>
    private float _elapsedResearchTime = 0f;

    /// <summary>
    /// Last second passed in research state.
    /// </summary>
    private int _lastResearchSecond = 0;

    /// <summary>
    /// Last minute passed in research state.
    /// </summary>
    private int _lastResearchMinute = 0;

    /// <summary>
    /// Value to control the chrono for the research.
    /// </summary>
    private bool _researchChronoIsRunning = false;
    #endregion

    #region Alerte
    /// <summary>
    /// Events to tell to all enemies when alerte state is ended.
    /// </summary>
    public event Action OnAlerteEnded;

    /// <summary>
    /// Elapsed time since the start of the alerte state.
    /// </summary>
    private float _elapsedAlerteTime = 0f;

    /// <summary>
    /// Last second passed in alerte state.
    /// </summary>
    private int _lastAlerteSecond = 0;

    /// <summary>
    /// Last minute passed in alerte state.
    /// </summary>
    private int _lastAlerteMinute = 0;

    /// <summary>
    /// Value to control the chrono for the alerte.
    /// </summary>
    private bool _alerteChronoIsRunning = false;
    #endregion

    #region Sound
    /// <summary>
    /// A dictionnary which stocks all sound sources currently heared by enemies.
    /// </summary>
    private readonly List<SoundSource> _soundSources = new();

    /// <summary>
    /// A locker to avoid that many instances can try to add, to sub, to unsub or invoke the same sound source at the same time.
    /// </summary>
    private readonly object s_addSourceLocker = new(), s_subSourceLocker = new(), s_unsubSourceLocker = new(), s_invokeSourceLocker = new();
    #endregion

    #region Vision
    /// <summary>
    /// A value indicating if the player is currently seen by at least one enemy.
    /// </summary>
    public bool PlayerIsCurrentlySeen { get; private set; }

    /// <summary>
    /// The last known player position.
    /// </summary>
    public PlayerPosition LastKnownPlayerPos;

    /// <summary>
    /// A dictionnary which stocks all player positions currently seen by enemies.
    /// </summary>
    private readonly List<PlayerPosition> _playerPositions = new();

    /// <summary>
    /// An event to indicate that the player position has been updated.
    /// </summary>
    public event Action<PlayerPosition> OnPlayerPosUpdated;

    /// <summary>
    /// A locker to avoid that many instances can try to update, to sub, to unsub or invoke the player position at the same time.
    /// </summary>
    private readonly object s_updatePlayerPosLocker = new(), s_subPlayerPosLocker = new(), s_unsubPlayerPosLocker = new(), s_invokePlayerPosLocker = new();
    #endregion

    private void Start()
    {
        for (int i = 0; i < EnemiesInRoom.Count; i++)
        {
            if (EnemiesInRoom[i] != null)
            {
                EnemiesInRoom[i].IsInNewRoom(this);
                AddAlerteLevelValue(EnemiesInRoom[i].CurrentAlerteLevel);
                UpdateRoomAlerteLevel();
                EnemiesInRoom[i].OnAlerteLevelChanged += ChangeAlerteLevel;
            }
        }

        for (int i = 0; i < _detectionObjects.Count; i++)
        {
            if (_detectionObjects[i] != null)
            {
                _detectionObjects[i].OnPlayerDetected += TryUpdatePlayerPos;
                _detectionObjects[i].OnPlayerDetected += GeneralAlerte;
            }
        }
    }

    private void Update()
    {
        // For research
        if (_researchChronoIsRunning && !EnemyManager.Instance.IsPaused)
        {
            // Decrement the elapsed time
            _elapsedResearchTime -= Time.deltaTime;

            // Check if the timer has reached 0
            if (_elapsedResearchTime <= 0f)
            {
                _elapsedResearchTime = 0f;
                StopResearchChrono();
                OnResearchEnded?.Invoke();
                return;
            }

            // Check seconds
            int currentSecond = (int)_elapsedResearchTime % 60;
            if (currentSecond != _lastResearchSecond)
            {
                _lastResearchSecond = currentSecond;

                // Check if it remains only 5 seconds
                if (RoomAlerteLevel == AlerteLevel.Research && _lastResearchMinute == 0 && _lastResearchSecond <= 5)
                {
                    OnAlerteAlmostFinished?.Invoke();
                }
            }

            // Check minutes
            int currentMinute = (int)(_elapsedResearchTime / 60);
            if (currentMinute != _lastResearchMinute)
            {
                _lastResearchMinute = currentMinute;
            }
        }

        // For alerte
        if (_alerteChronoIsRunning && !EnemyManager.Instance.IsPaused)
        {
            // Decrement the elapsed time
            _elapsedAlerteTime -= Time.deltaTime;

            // Check if the timer has reached 0
            if (_elapsedAlerteTime <= 0f)
            {
                _elapsedAlerteTime = 0f;
                StopAlerteChrono();
                OnAlerteEnded?.Invoke();

                // Launch research timer after alerte
                StartResearchChrono(EnemyManager.Instance.ResearchTimerAfterAlerte);
                return;
            }

            // Check seconds
            int currentSecond = (int)_elapsedAlerteTime % 60;
            if (currentSecond != _lastAlerteSecond)
            {
                _lastAlerteSecond = currentSecond;

                // Check if it remains only 5 seconds
                if (RoomAlerteLevel == AlerteLevel.Alerte && _lastAlerteMinute == 0 && _lastAlerteSecond <= 5)
                {
                    OnAlerteAlmostFinished?.Invoke();
                }
            }

            // Check minutes
            int currentMinute = (int)(_elapsedAlerteTime / 60);
            if (currentMinute != _lastAlerteMinute)
            {
                _lastAlerteMinute = currentMinute;
            }
        }
    }

    public void OnDisable()
    {
        _enemiesAlerteLevels.Clear();
        _enemiesAlerteLevels = new() { { AlerteLevel.Patrol, 0 }, { AlerteLevel.Research, 0 }, { AlerteLevel.Alerte, 0 } };
        RoomAlerteLevel = AlerteLevel.Patrol;
        ResetResearchChrono();
        ResetAlerteChrono();
    }

    #region Room
    /// <summary>
    /// Called to try to add the enemy in the room.
    /// </summary>
    /// <param name="enemy"> The enemy to add. </param>
    public void TryAddEnemy(EnemyBrain enemy)
    {
        if (!EnemiesInRoom.Contains(enemy))
        {
            EnemiesInRoom.Add(enemy);
            enemy.IsInNewRoom(this);
            AddAlerteLevelValue(enemy.CurrentAlerteLevel);
            UpdateRoomAlerteLevel();
            enemy.OnAlerteLevelChanged += ChangeAlerteLevel;
        }
    }

    /// <summary>
    /// Called to try to remove the enemy of the room.
    /// </summary>
    /// <param name="enemy"> The enemy to remove. </param>
    public void TryRemoveEnemy(EnemyBrain enemy)
    {
        if (EnemiesInRoom.Contains(enemy))
        {
            EnemiesInRoom.Remove(enemy);
            RemoveAlerteLevelValue(enemy.CurrentAlerteLevel);
            UpdateRoomAlerteLevel();
            enemy.OnAlerteLevelChanged -= ChangeAlerteLevel;
        }
    }

    /// <summary>
    /// Called to try add the player in the room.
    /// </summary>
    /// <param name="player"> The player to add. </param>
    public void AddPlayer(PlayerStateManager player)
    {
        _playerInRoom = player;
        OnPlayerIsInTheRoom?.Invoke(this);
        _playerInRoom.IsInNewRoom(this);
    }

    /// <summary>
    /// Called to remove the player of the room.
    /// </summary>
    public void RemovePlayer()
    {
        _playerInRoom = null;
    }

    /// <summary>
    /// Callled to change alerte levels values when an enemy changes state.
    /// </summary>
    /// <param name="oldAlerteLevel"> The old alerte level of the enemy. </param>
    /// <param name="newAlerteLevel"> The new Alerte level of the enemy. </param>
    private void ChangeAlerteLevel(AlerteLevel oldAlerteLevel, AlerteLevel newAlerteLevel)
    {
        RemoveAlerteLevelValue(oldAlerteLevel);
        AddAlerteLevelValue(newAlerteLevel);
        UpdateRoomAlerteLevel();
    }

    /// <summary>
    /// Called to add an alerte level.
    /// </summary>
    /// <param name="alerteLevel"> Alerte level to add. </param>
    private void AddAlerteLevelValue(AlerteLevel alerteLevel)
    {
        lock (s_addAlerteLevelLocker)
        {
            _enemiesAlerteLevels[alerteLevel] += 1;
        }
    }

    /// <summary>
    /// Called to remove an alerte level.
    /// </summary>
    /// <param name="alerteLevel"> Alerte level to remove. </param>
    private void RemoveAlerteLevelValue(AlerteLevel alerteLevel)
    {
        lock (s_removeAlerteLevelLocker)
        {
            _enemiesAlerteLevels[alerteLevel] -= 1;
        }
    }

    /// <summary>
    /// Called to update the room alerte level depending of the value of each level.
    /// </summary>
    private void UpdateRoomAlerteLevel()
    {
        AlerteLevel currentRoomAlerteLevel = RoomAlerteLevel;

        if (_enemiesAlerteLevels[AlerteLevel.Alerte] > 0)
        {
            RoomAlerteLevel = AlerteLevel.Alerte;
        }
        else if (_enemiesAlerteLevels[AlerteLevel.Research] > 0)
        {
            RoomAlerteLevel = AlerteLevel.Research;
        }
        else
        {
            RoomAlerteLevel = AlerteLevel.Patrol;
        }

        if (currentRoomAlerteLevel != RoomAlerteLevel)
        {
            OnRoomAlerteLevelChanged?.Invoke(RoomAlerteLevel);
        }
    }

    /// <summary>
    /// Called to trigger the generale alerte.
    /// </summary>
    private void GeneralAlerte(Vector3 ignore, PlayerSeenContext context)
    {
        if (context == PlayerSeenContext.Continue || context == PlayerSeenContext.LastTime) return;

        if (_isThereAlreadyGeneralAlerte) return;

        _isThereAlreadyGeneralAlerte = true;

        _alarmeLight.SetActive(true);

        OnGeneralAlerte?.Invoke(AlerteLevel.GeneralAlerte);

        for (int i = 0; i < EnemiesInRoom.Count; i++)
        {
            EnemiesInRoom[i].GeneralAlerte();
        }
    }
    #endregion

    #region Chrono
    /// <summary>
    /// Called to start the research chrono.
    /// </summary>
    /// <param name="startTime"> Started time of the chrono (in secondes). </param>
    public void StartResearchChrono(float startTime)
    {
        ResetResearchChrono();

        _elapsedResearchTime = startTime;

        _researchChronoIsRunning = true;
    }

    /// <summary>
    /// Called to stop the research chrono.
    /// </summary>
    public void StopResearchChrono()
    {
        _researchChronoIsRunning = false;
    }

    /// <summary>
    /// Called to reset the research chrono.
    /// </summary>
    public void ResetResearchChrono()
    {
        _elapsedResearchTime = 0f;
        _lastResearchSecond = 0;
        _lastResearchMinute = 0;
    }

    /// <summary>
    /// Called to start the alerte chrono.
    /// </summary>
    /// <param name="startTime"> Started time of the chrono (in secondes). </param>
    public void StartAlerteChrono(float startTime)
    {
        ResetAlerteChrono();

        _elapsedAlerteTime = startTime;

        _alerteChronoIsRunning = true;
    }

    /// <summary>
    /// Called to stop the alerte chrono.
    /// </summary>
    public void StopAlerteChrono()
    {
        _isThereAlreadyGeneralAlerte = false;
        _alerteChronoIsRunning = false;
        _alarmeLight.SetActive(false);
    }

    /// <summary>
    /// Called to reset the alerte chrono.
    /// </summary>
    public void ResetAlerteChrono()
    {
        _elapsedAlerteTime = 0f;
        _lastAlerteSecond = 0;
        _lastAlerteMinute = 0;
    }
    #endregion

    #region Sound
    /// <summary>
    /// Called to subscribe to the event of a sound source. 
    /// </summary>
    /// <param name="source"> The source of the sound. </param>
    /// <param name="callback"> The action to perform when the event of the source is triggered. </param>
    public void SubscribeSoundSource(SoundSource source, Action callback)
    {
        lock (s_subSourceLocker)
        {
            if (source == null) return;

            source.OnReached += callback;
            source.Listeners += 1;

            if (!_soundSources.Contains(source))
            {
                _soundSources.Add(source);
            }
        }
    }

    /// <summary>
    /// Called to unsubscribe to the event of a sound source.
    /// </summary>
    /// <param name="source"> The source of the sound. </param>
    /// <param name="callback"> The action to perform when the event of the source is triggered. </param>
    public void UnsubscribeSoundSource(SoundSource source, Action callback)
    {
        lock (s_unsubSourceLocker)
        {
            if (source == null) return;

            if (_soundSources.Contains(source))
            {
                source.OnReached -= callback;
                source.Listeners -= 1;
                if (source.Listeners <= 0)
                {
                    _soundSources.Remove(source);
                }
            }
        }
    }

    /// <summary>
    /// Called to trigger the event of a sound source.
    /// </summary>
    /// <param name="source"> The source to trigger. </param>
    /// <param name="callback"> The action to unsubscribe from the event invoked. </param>
    public void InvokeSoundSource(SoundSource source, Action callback)
    {
        lock (s_invokeSourceLocker)
        {
            if (source == null) return;
            UnsubscribeSoundSource(source, callback);
            if (_soundSources.Contains(source))
            {
                source?.Invoke();
            }
        }
    }
    #endregion

    #region Vision
    /// <summary>
    /// Called to try to update the last known player position.
    /// </summary>
    /// <param name="position"> Player position. </param>
    /// <param name="playerSeenContext"> Context of the vision. </param>
    public void TryUpdatePlayerPos(Vector3 position, PlayerSeenContext playerSeenContext)
    {
        lock (s_updatePlayerPosLocker)
        {
            PlayerIsCurrentlySeen = playerSeenContext == PlayerSeenContext.Continue || playerSeenContext == PlayerSeenContext.FirstTime;

            if (LastKnownPlayerPos != null)
            {
                if (position != LastKnownPlayerPos.Position)
                {
                    LastKnownPlayerPos = new(position, playerSeenContext);
                    OnPlayerPosUpdated?.Invoke(LastKnownPlayerPos);
                }
            }
            else
            {
                LastKnownPlayerPos = new(position, playerSeenContext);
                OnPlayerPosUpdated?.Invoke(LastKnownPlayerPos);
            }
        }
    }

    /// <summary>
    /// Called to subscribe to the event of the last player position. 
    /// </summary>
    /// <param name="callback"> The action to perform when the event of the position is triggered. </param>
    public void SubscribePlayerPos(Action callback)
    {
        lock (s_subPlayerPosLocker)
        {
            if (LastKnownPlayerPos != null)
            {
                LastKnownPlayerPos.OnReached += callback;
                LastKnownPlayerPos.Listeners += 1;

                if (!_playerPositions.Contains(LastKnownPlayerPos))
                {
                    _playerPositions.Add(LastKnownPlayerPos);
                }
            }
        }
    }

    /// <summary>
    /// Called to unsubscribe to the event of the last player position. 
    /// </summary>
    /// <param name="position"> The player position to unsubscribe. </param>
    /// <param name="callback"> The action to perform when the event of the source is triggered. </param>
    public void UnsubscribePlayerPos(PlayerPosition position, Action callback)
    {
        lock (s_unsubPlayerPosLocker)
        {
            if (position == null) return;

            if (_playerPositions.Contains(position))
            {
                position.OnReached -= callback;
                position.Listeners -= 1;
                if (position.Listeners <= 0)
                {
                    _playerPositions.Remove(position);
                }
            }
        }
    }

    /// <summary>
    /// Called to trigger the event of a player position.
    /// </summary>
    /// <param name="position"> The player position to invoke. </param>
    /// <param name="callback"> The action to unsubscribe from the event invoked. </param>
    public void InvokePlayerPos(PlayerPosition position, Action callback)
    {
        lock (s_invokePlayerPosLocker)
        {
            if (position == null) return;
            UnsubscribePlayerPos(position, callback);
            if (_playerPositions.Contains(position))
            {
                position?.Invoke();
            }
        }
    }
    #endregion
}
