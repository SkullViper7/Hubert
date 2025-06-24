using UnityEngine;

public class CameraLight : MonoBehaviour
{
    /// <summary>
    /// Material when enemy is in patrol state.
    /// </summary>
    [SerializeField]
    private Material _patrolMaterial;

    /// <summary>
    /// Material when enemy is in alerte state.
    /// </summary>
    [SerializeField]
    private Material _alerteMaterial;

    /// <summary>
    /// The vision component.
    /// </summary>
    [SerializeField]
    private EnemyVision _enemyVision;

    /// <summary>
    /// Mesh renderer of the light.
    /// </summary>
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _meshRenderer.material = _patrolMaterial;

        _enemyVision.OnPlayerSeen += ChangeLightColor;
    }

    /// <summary>
    /// Called to change the color of the light when the camera sees the player or not.
    /// </summary>
    /// <param name="context"> The context of the vision. </param>
    private void ChangeLightColor(Vector3 ignore, PlayerSeenContext context)
    {
        if (context == PlayerSeenContext.FirstTime || context == PlayerSeenContext.Continue)
        {
            _meshRenderer.material = _alerteMaterial;
        }
        else if (context == PlayerSeenContext.LastTime)
        {
            _meshRenderer.material = _patrolMaterial;
        }
    }
}
