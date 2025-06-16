using System.Collections.Generic;
using UnityEngine;

public class SkinDisplayer : MonoBehaviour
{
    /// <summary>
    /// All possible skins for enemies.
    /// </summary>
    [SerializeField]
    private List<Material> _skins;

    /// <summary>
    /// The rarest skin for enemies.
    /// </summary>
    [SerializeField]
    private Material _shinySkin;

    /// <summary>
    /// Stars particles to activate when it's a shiny enemy.
    /// </summary>
    [SerializeField]
    private GameObject _shinyParticles;

    /// <summary>
    /// The probability for the enemy of being shiny. (1 on this value)
    /// </summary>
    [SerializeField]
    private int _shinyProbability;

    /// <summary>
    /// Mesh renderer component of the enemy.
    /// </summary>
    private SkinnedMeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        if (Random.Range(0, _shinyProbability) == 0)
        {
            _meshRenderer.material = _shinySkin;
            _shinyParticles.SetActive(true);
        }
        else
        {
            _meshRenderer.material = _skins[Random.Range(0, _skins.Count)];
        }
    }
}
