using UnityEngine;

[ExecuteInEditMode]
public class ModularLaser : MonoBehaviour
{
    [SerializeField] Transform _box1;
    [SerializeField] Transform _laser;

    [SerializeField] float _laserThickness;

    [SerializeField] LayerMask _layerMask;

    void Update()
    {
        if (Physics.Raycast(_box1.position, _box1.forward, out RaycastHit hit, 100f, _layerMask))
        {
            Vector3 distanceBetweenBoxes = hit.point - _box1.position;

            _laser.localScale = new Vector3(_laserThickness, distanceBetweenBoxes.magnitude / 2, _laserThickness); ;
            _laser.position = (_box1.position + hit.point) * 0.5f;
            _laser.up = distanceBetweenBoxes.normalized;
        }
        else
        {
            _laser.localScale = new Vector3(_laserThickness, 1000, _laserThickness);
            _laser.position = _box1.position + _box1.forward * 1000;
        }
    }
}
