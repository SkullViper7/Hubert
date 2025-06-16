using Cinemachine;
using UnityEngine;

public class HubertImpulseSource : MonoBehaviour
{
    CinemachineImpulseSource _impulseSource;

    void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void FootstepShake()
    {
        Shake(new Vector3(0f, -0.03f, 0f), 0.2f);
    }

    public void ShootShake()
    {
        Shake(new Vector3(0f, -0.5f, 0f), 0.2f);
    }

    public void HitShake()
    {
        Shake(new Vector3(0f, -0.5f, 0f), 0.2f);
    }

    void Shake(Vector3 velocity, float duration)
    {
        // Set the default velocity, impulse duration, and generate the impulse
        _impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        _impulseSource.m_DefaultVelocity = velocity;
        _impulseSource.GenerateImpulse();
    }
}
