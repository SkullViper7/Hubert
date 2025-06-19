using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class ModularLaser : MonoBehaviour
{
    /// <summary>
    /// Start position of the laser.
    /// </summary>
    [SerializeField] 
    private Transform _start;

    /// <summary>
    /// The laser mesh object.
    /// </summary>
    [SerializeField] 
    private Transform _laserMesh;

    /// <summary>
    /// The laser collider object.
    /// </summary>
    [SerializeField]
    private Transform _laserCollider;

    /// <summary>
    /// Thickness of the laser.
    /// </summary>
    [SerializeField] 
    private float _laserThickness;

    /// <summary>
    /// Layer mask of the mesh.
    /// </summary>
    [SerializeField] 
    private LayerMask _meshLayerMask;

    /// <summary>
    /// Layer mask of the collider.
    /// </summary>
    [SerializeField]
    private LayerMask _colliderLayerMask;

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (!Application.isPlaying)
        {
            UpdateLaser();
        }
    }
#endif

    private void FixedUpdate()
    {
        if (Application.isPlaying)
            UpdateLaser();
    }

    private void UpdateLaser()
    {
        // Cut the mesh laser
        if (Physics.Raycast(_start.position, _start.forward, out RaycastHit meshHit, 100f, _meshLayerMask))
        {
            Vector3 distanceBetweenBoxes = meshHit.point - _start.position;

            _laserMesh.localScale = new Vector3(_laserThickness, distanceBetweenBoxes.magnitude / 2, _laserThickness); ;
            _laserMesh.position = (_start.position + meshHit.point) * 0.5f;
            _laserMesh.up = distanceBetweenBoxes.normalized;
        }
        else
        {
            _laserMesh.localScale = new Vector3(_laserThickness, 1000, _laserThickness);
            _laserMesh.position = _start.position + _start.forward * 1000;
        }

        // Cut the collider laser
        if (Physics.Raycast(_start.position, _start.forward, out RaycastHit colliderHit, 100f, _colliderLayerMask))
        {
            Vector3 distanceBetweenBoxes = colliderHit.point - _start.position;

            _laserCollider.localScale = new Vector3(_laserThickness, distanceBetweenBoxes.magnitude / 2, _laserThickness); ;
            _laserCollider.position = (_start.position + colliderHit.point) * 0.5f;
            _laserCollider.up = distanceBetweenBoxes.normalized;
        }
        else
        {
            _laserCollider.localScale = new Vector3(_laserThickness, 1000, _laserThickness);
            _laserCollider.position = _start.position + _start.forward * 1000;
        }
    }
}
