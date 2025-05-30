using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Singleton
    private static EnemyManager _instance = null;
    public static EnemyManager Instance => _instance;

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
    /// The global time of the alerte state when nobody has seen the player for a while.
    /// </summary>
    [field: SerializeField, Space, Header("Alerte")]
    public float AlerteTimer { get; private set; }

    /// <summary>
    /// A value to check if the chrono is paused.
    /// </summary>
    public bool IsPaused { get; private set; }

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

    /// <summary>
    /// Called to pause all chronos.
    /// </summary>
    public void PauseChronos()
    {
        IsPaused = true;
    }

    /// <summary>
    /// Called to resume all chronos.
    /// </summary>
    public void ResumeChrono()
    {
        IsPaused = false;
    }
}
