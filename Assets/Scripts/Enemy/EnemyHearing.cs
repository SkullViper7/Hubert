using System;
using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    /// <summary>
    /// An event triggered when a sound is heard by the ears.
    /// </summary>
    public event Action<SoundSource> OnSoundHeard;

    public void HearSound(SoundSource soundSource)
    {
        OnSoundHeard?.Invoke(soundSource);
    }
}
