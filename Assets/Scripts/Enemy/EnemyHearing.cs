using System;
using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    /// <summary>
    /// Cooldown to switch the first continue sound into a one shot sound.
    /// </summary>
    [SerializeField]
    private float _hearingCooldown = 1.5f;

    [SerializeField]
    private float _timer = 0f;

    [SerializeField]
    private bool _isTimerRunning = false;

    /// <summary>
    /// An event triggered when a sound is heard by the ears.
    /// </summary>
    public event Action<SoundSource> OnSoundHeard;

    private void Update()
    {
        if (_isTimerRunning)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isTimerRunning = false;
                _timer = 0f;
            }
        }
    }

    /// <summary>
    /// Called when a sound is heared.
    /// </summary>
    /// <param name="soundSource"> The sound source heared. </param>
    public void HearSound(SoundSource soundSource)
    {
        if (soundSource.SoundType == SoundType.Continue)
        {
            if (!_isTimerRunning)
            {
                soundSource.SoundType = SoundType.OneShot;
            }

            // Restart the timer
            _timer = _hearingCooldown;
            _isTimerRunning = true;
        }
        OnSoundHeard?.Invoke(soundSource);
    }
}
