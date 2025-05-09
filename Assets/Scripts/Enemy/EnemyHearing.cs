using System;
using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    /// <summary>
    /// An event triggered when a sound is heard by the ears.
    /// </summary>
    public event Action<Vector3> OnSoundHeard;

    public void HearSound(Vector3 soundPosition)
    {
        OnSoundHeard?.Invoke(soundPosition);
        Debug.Log(soundPosition);
    }
}
