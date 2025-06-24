using Cinemachine;
using UnityEngine;

public class HubertImpulseSource : MonoBehaviour
{
    /// <summary>
    /// Animation controller of the player.
    /// </summary>
    private PlayerAnimationController _animationController;

    /// <summary>
    /// Cinemachine impulse source component.
    /// </summary>
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _animationController = GetComponent<PlayerAnimationController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        _animationController.OnLeftStep += FootstepShake;
        _animationController.OnRightStep += FootstepShake;
        _animationController.MustShoot += ShootShake;
        _animationController.MustHit += HitShake;
        _animationController.OnStartElectrified += ElectrifiedShake;
        _animationController.OnFall += FallShake;
    }

    private void FootstepShake()
    {
        Shake(new Vector3(0f, -0.03f, 0f), 0.2f);
    }

    private void ShootShake()
    {
        Shake(new Vector3(0f, -0.5f, 0f), 0.2f);
    }

    private void HitShake()
    {
        Shake(new Vector3(0f, -0.5f, 0f), 0.2f);
    }

    private void ElectrifiedShake()
    {
        ImpulseManager.Instance.Impulse();
    }

    private void FallShake()
    {
        Shake(new Vector3(0f, -0.25f, 0f), 0.2f);
    }

    /// <summary>
    /// Called to generate an impulse to shake the camera.
    /// </summary>
    /// <param name="velocity"> Velocity of the impulse. </param>
    /// <param name="duration"> Duration of the impulse. </param>
    private void Shake(Vector3 velocity, float duration)
    {
        // Set the default velocity, impulse duration, and generate the impulse
        _impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        _impulseSource.m_DefaultVelocity = velocity;
        _impulseSource.GenerateImpulse();
    }
}
