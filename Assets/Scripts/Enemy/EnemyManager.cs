using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyManager : MonoBehaviour
{
    // Singleton
    private static EnemyManager _instance = null;
    public static EnemyManager Instance => _instance;

    #region Research
    /// <summary>
    /// The global time of the research state when nobody has seen the player for a while.
    /// </summary>
    [field: SerializeField, Space, Header("Research")]
    public float ResearchTimer { get; private set; }

    /// <summary>
    /// The global time of the research state after the alerte state.
    /// </summary>
    [field: SerializeField]
    public float ResearchTimerAfterAlerte { get; private set; }

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

    /// <summary>
    /// A dictionnary which stocks all sound sources currently heared by enemies and an event for each sound source when it will be checked by an enemy.
    /// </summary>
    private Dictionary<SoundSource, Action> _soundSources = new();

    /// <summary>
    /// A locker to avoid that many instances can try to add the same sound source or can invoke the same event at the same time.
    /// </summary>
    private static readonly object s_addLocker = new(), s_invokeLocker = new();
    #endregion

    #region Alerte
    /// <summary>
    /// The global time of the alerte state when nobody has seen the player for a while.
    /// </summary>
    [field: SerializeField, Space, Header("Alerte")]
    public float AlerteTimer { get; private set; }

    /// <summary>
    /// An event to tell to all enemies in alert state where is the player if one enemy has seen him.
    /// </summary>
    public event Action<Vector3> OnPlayerSeen;

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

    /// <summary>
    /// Variable to check if the chrono is paused.
    /// </summary>
    private bool _isPaused = false;

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

    private void Update()
    {
        // For research
        if (_researchChronoIsRunning && !_isPaused)
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
        else
        {
            return;
        }

        // For alerte
        if (_alerteChronoIsRunning && !_isPaused)
        {
            // Decrement the elapsed time
            _elapsedAlerteTime -= Time.deltaTime;

            // Check if the timer has reached 0
            if (_elapsedAlerteTime <= 0f)
            {
                _elapsedAlerteTime = 0f;
                StopAlerteChrono();
                OnAlerteEnded?.Invoke();
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
        else
        {
            return;
        }
    }

    /// <summary>
    /// Called to try to add a sound source in the dictionnary.
    /// </summary>
    /// <param name="source"></param>
    public SoundSource TryAddSound(SoundSource source)
    {
        lock (s_addLocker)
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
    /// <param name="callback"> The event associated to the sound source. </param>
    public void Subscribe(SoundSource source, Action callback)
    {
        if (_soundSources.TryGetValue(source, out var action))
        {
            _soundSources[source] += callback;
        }
    }

    /// <summary>
    /// Called to trigger the event of a sound source.
    /// </summary>
    /// <param name="source"> The source to trigger. </param>
    public void Invoke(SoundSource source)
    {
        lock (s_invokeLocker)
        {
            if (_soundSources.TryGetValue(source, out var action))
            {
                action?.Invoke();
                _soundSources.Remove(source);
            }
        }
    }

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
        _researchChronoIsRunning = false;
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
        _alerteChronoIsRunning = false;
        _elapsedAlerteTime = 0f;
        _lastAlerteSecond = 0;
        _lastAlerteMinute = 0;
    }

    /// <summary>
    /// Called to pause all chronos.
    /// </summary>
    public void PauseChronos()
    {
        _isPaused = true;
    }

    /// <summary>
    /// Called to resume all chronos.
    /// </summary>
    public void ResumeChrono()
    {
        _isPaused = false;
    }
}
