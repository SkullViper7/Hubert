using Cinemachine;
using UnityEngine;

public class ImpulseManager : MonoBehaviour
{
    private static ImpulseManager _instance;
    public static ImpulseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ImpulseManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("ImpulseManager");
                    _instance = obj.AddComponent<ImpulseManager>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    CinemachineImpulseSource _impulseSource;

    // Singleton instance of ImpulseManager.
    // Ensures there is only one instance of ImpulseManager in the scene.
    private void Awake()
    {
        // If this is not the instance of ImpulseManager, destroy this object.
        if (this != Instance)
        {
            Destroy(this.gameObject);
        }

        // Get the CinemachineImpulseSource component attached to this game object.
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// Shakes the camera using the Cinemachine Impulse Source.
    /// Type : 0 = Uniform, 1 = Dissipating, 2 = Propagating.
    /// Shape : 1 = Recoil, 2 = Bump, 3 = Explosion, 4 = Rumble.
    /// </summary>
    /// <param name="type">The type of impulse to use.</param>
    /// <param name="shape">The shape of the impulse.</param>
    /// <param name="duration">The duration of the impulse.</param>
    public void Shake(int type, int shape, Vector3 velocity, float duration)
    {
        // Set the impulse type based on the 'type' parameter using a switch statement
        switch (type)
        {
            case 0: // Uniform
                _impulseSource.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
                break;
            case 1: // Dissipating
                _impulseSource.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Dissipating;
                break;
            case 2: // Propagating
                _impulseSource.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Propagating;
                break;
            case 3: // Legacy
                _impulseSource.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Legacy;
                break;
        }

        // Set the impulse shape based on the 'shape' parameter using a switch statement
        switch (shape)
        {
            case 0: // Custom
                _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
                break;
            case 1: // Recoil
                _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Recoil;
                break;
            case 2: // Bump
                _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump;
                break;
            case 3: // Explosion
                _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion;
                break;
            case 4: // Rumble
                _impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble;
                break;
        }

        // Set the default velocity, impulse duration, and generate the impulse
        _impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        _impulseSource.m_DefaultVelocity = velocity;
        _impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// Generates an impulse without specifying the type, shape, velocity, or duration.
    /// </summary>
    public void Impulse()
    {
        // Generate an impulse using the Cinemachine Impulse Source.
        // The impulse type, shape, default velocity, and duration are set in the Shake method.
        _impulseSource.GenerateImpulse();
    }
}
