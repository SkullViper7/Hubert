using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] float _bpm;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Intervals[] _intervals;

    void Update()
    {
        for (int i = 0; i < _intervals.Length; i++)
        {
            float sampledTime = _audioSource.timeSamples / (_audioSource.clip.frequency * _intervals[i].GetIntervalLength(_bpm));
            _intervals[i].CheckForNewInterval(sampledTime);
        }
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] float _steps;
    [SerializeField] UnityEvent _trigger;
    int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    } 

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}
