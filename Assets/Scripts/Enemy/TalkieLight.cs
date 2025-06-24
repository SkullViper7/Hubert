using UnityEngine;

public class TalkieLight : MonoBehaviour
{
    /// <summary>
    /// Material when enemy is in patrol state.
    /// </summary>
    [SerializeField]
    private Material _patrolMaterial;

    /// <summary>
    /// Material when enemy is in research state.
    /// </summary>
    [SerializeField]
    private Material _researchMaterial;

    /// <summary>
    /// Material when enemy is in alerte state.
    /// </summary>
    [SerializeField]
    private Material _alerteMaterial;

    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    [SerializeField]
    private EnemyBrain _enemyBrain;

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

        _enemyBrain.OnAlerteLevelChanged += ChangeLightColor;
    }

    /// <summary>
    /// Called to change the color of the light depending of the alerte level.
    /// </summary>
    /// <param name="oldAlerteLevel"> Old alerte level of the enemy. </param>
    /// <param name="newAlerteLevel"> New alerte level of the enemy. </param>
    private void ChangeLightColor(AlerteLevel oldAlerteLevel, AlerteLevel newAlerteLevel)
    {
        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                _meshRenderer.material = _patrolMaterial;
                break;
            case AlerteLevel.Research:
                _meshRenderer.material = _researchMaterial;
                break;
            case AlerteLevel.Alerte:
                _meshRenderer.material = _alerteMaterial;
                break;
        }
    }
}
