using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region Room
    [SerializeField]
    private List<EnemyBrain> _enemiesInRoom;
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
    [SerializeField]
    private int _lastResearchSecond = 0;

    /// <summary>
    /// Last minute passed in research state.
    /// </summary>
    [SerializeField]
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
    [SerializeField]
    private int _lastAlerteSecond = 0;

    /// <summary>
    /// Last minute passed in alerte state.
    /// </summary>
    [SerializeField]
    private int _lastAlerteMinute = 0;

    /// <summary>
    /// Value to control the chrono for the alerte.
    /// </summary>
    private bool _alerteChronoIsRunning = false;
    #endregion

    #region Sound
    /// <summary>
    /// A dictionnary which stocks all sound sources currently heared by enemies and an event for each sound source when it will be checked by an enemy.
    /// </summary>
    private Dictionary<SoundSource, Action> _soundSources = new();

    /// <summary>
    /// A locker to avoid that many instances can try to add the same sound source or can invoke the same event at the same time.
    /// </summary>
    private static readonly object s_addSourceLocker = new(), s_invokeSourceLocker = new();
    #endregion

    #region Vision
    /// <summary>
    /// The last known player position.
    /// </summary>
    public PlayerPosition LastKnownPlayerPos;

    /// <summary>
    /// An event to indicate that the player position has been updated.
    /// </summary>
    public event Action<PlayerPosition> OnPlayerPosUpdated;

    /// <summary>
    /// Static id to set a unique ID to each update of the player position.
    /// </summary>
    private static int s_PlayerPositionID;

    /// <summary>
    /// A locker to avoid that many instances can try to update the position at the same time.
    /// </summary>
    private static readonly object s_updatePlayerPosLocker = new();
    #endregion

    private void Start()
    {
        for (int i = 0; i < _enemiesInRoom.Count; i++)
        {
            if (_enemiesInRoom[i] != null)
            {
                _enemiesInRoom[i].IsInNewRoom(this);
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
            }

            // Check minutes
            int currentMinute = (int)(_elapsedAlerteTime / 60);
            if (currentMinute != _lastAlerteMinute)
            {
                _lastAlerteMinute = currentMinute;
            }
        }
    }

    #region Room
    /// <summary>
    /// Called to try to add the enemy in the room.
    /// </summary>
    /// <param name="enemy"> The enemy to add. </param>
    public void TryAddEnemy(EnemyBrain enemy)
    {
        if (!_enemiesInRoom.Contains(enemy))
        {
            _enemiesInRoom.Add(enemy);
            enemy.IsInNewRoom(this);
        }
    }

    /// <summary>
    /// Called to try to remove the enemy of the room.
    /// </summary>
    /// <param name="enemy"> The enemy to remove. </param>
    public void TryRemoveEnemy(EnemyBrain enemy)
    {
        if (_enemiesInRoom.Contains(enemy))
        {
            _enemiesInRoom.Remove(enemy);
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
        _alerteChronoIsRunning = false;
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
    /// Called to try to add a sound source in the dictionnary.
    /// </summary>
    /// <param name="source"></param>
    public SoundSource TryAddSound(SoundSource source)
    {
        lock (s_addSourceLocker)
        {
            if (!_soundSources.ContainsKey(source))
            {
                _soundSources[source] = () => { };
            }
            return source;
        }
    }

    /// <summary>
    /// Called to subscribe to the event of a sound source. 
    /// </summary>
    /// <param name="source"> The source of the sound. </param>
    /// <param name="callback"> The action to perform when the event of the source is triggered. </param>
    public void Subscribe(SoundSource source, Action callback)
    {
        if (_soundSources.TryGetValue(source, out var action))
        {
            _soundSources[source] += callback;
            source.Listeners += 1;
        }
    }

    /// <summary>
    /// Called to unsubscribe to the event of a sound source.
    /// </summary>
    /// <param name="source"> The source of the sound. </param>
    /// <param name="callback"> The action to perform when the event of the source is triggered. </param>
    public void Unsubscribe(SoundSource source, Action callback)
    {
        if (_soundSources.TryGetValue(source, out var action))
        {
            _soundSources[source] -= callback;
            source.Listeners -= 1;
            if (source.Listeners <= 0)
            {
                _soundSources.Remove(source);
            }
        }
    }

    /// <summary>
    /// Called to trigger the event of a sound source.
    /// </summary>
    /// <param name="source"> The source to trigger. </param>
    public void Invoke(SoundSource source)
    {
        lock (s_invokeSourceLocker)
        {
            if (_soundSources.TryGetValue(source, out var action))
            {
                action?.Invoke();
                _soundSources.Remove(source);
            }
        }
    }
    #endregion

    #region Vision
    /// <summary>
    /// Called to try to update the last known player position.
    /// </summary>
    /// <param name="position"> Player position. </param>
    public void TryUpdatePlayerPos(Vector3 position)
    {
        lock (s_updatePlayerPosLocker)
        {
            if (position != LastKnownPlayerPos.Position)
            {
                LastKnownPlayerPos = new(s_PlayerPositionID++, position, PlayerSeenContext.Continue);
                OnPlayerPosUpdated?.Invoke(LastKnownPlayerPos);
            }
        }
    }
    #endregion
}
