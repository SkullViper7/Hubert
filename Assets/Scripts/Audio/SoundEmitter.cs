using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    /// <summary>
    /// Called to emit a sound at a position and with a radius.
    /// </summary>
    /// <param name="soundPosition"> Position of the sound. </param>
    /// <param name="soundRadius"> Radius in which enemy can hear. </param>
    public void EmitSound(Vector3 soundPosition, float soundRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius, LayerMask.GetMask("EnemyEars"));

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyEars>(out EnemyEars enemyEars))
            {
                enemyEars.HearSound(soundPosition);
            }
        }
    }
}
