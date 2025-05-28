using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private static int s_SoundID;

    /// <summary>
    /// Called to emit a sound at a position and with a radius.
    /// </summary>
    /// <param name="soundPosition"> Position of the sound. </param>
    /// <param name="soundRadius"> Radius in which enemy can hear. </param>
    public void EmitSound(Vector3 soundPosition, float soundRadius)
    {
        SoundSource soundSource = new(s_SoundID++, soundPosition);

        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius, LayerMask.GetMask("EnemyEars"));

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyHearing>(out EnemyHearing enemyEars))
            {
                enemyEars.HearSound(soundSource); 
            }
        }
    }
}
