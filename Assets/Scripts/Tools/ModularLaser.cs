using UnityEngine;

[ExecuteInEditMode]
public class ModularLaser : MonoBehaviour
{
    [SerializeField] Transform _box1;
    [SerializeField] Transform _box2;
    [SerializeField] Transform _laser;

    [SerializeField] float _laserThickness;

    
    void Update()
    {
        Vector3 distanceBetweenBoxes = _box2.position - _box1.position;
        _laser.localScale = new Vector3(_laserThickness, distanceBetweenBoxes.magnitude/2,_laserThickness);
        _laser.position = (_box1.position + _box2.position) * 0.5f;
        _laser.up = distanceBetweenBoxes.normalized;
    }
}
