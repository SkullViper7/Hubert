using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmIKManager : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint _leftArmIK;
    [SerializeField] TwoBoneIKConstraint _rightArmIK;

    [SerializeField] Transform _leftRaycastOrigin;
    [SerializeField] Transform _rightRaycastOrigin;
    [SerializeField] Transform _leftRigTarget;
    [SerializeField] Transform _rightRigTarget;

    [SerializeField] LayerMask _wallMask;

    [SerializeField] float _maxDistance = 1f;
    [SerializeField] float _lerpSpeed = 5f;

    float _leftCurrentWeight;
    float _rightCurrentWeight;

    void FixedUpdate()
    {
        // Left arm
        if (Physics.Raycast(_leftRaycastOrigin.position, _leftRaycastOrigin.forward, out RaycastHit hit, _maxDistance, _wallMask))
        {
            _leftRigTarget.position = hit.point + hit.normal * 0.15f;
            //_leftRigTarget.rotation = Quaternion.LookRotation(-hit.normal);

            // Calcule le poids en fonction de la distance
            float t = 1f - Mathf.Clamp01(hit.distance / _maxDistance);
            float targetWeight = Mathf.SmoothStep(0f, 1f, t);

            // Interpolation fluide du poids
            _leftCurrentWeight = Mathf.Lerp(_leftCurrentWeight, targetWeight, Time.deltaTime * _lerpSpeed);
        }
        else
        {
            // Loin du mur → retour à 0
            _leftCurrentWeight = Mathf.Lerp(_leftCurrentWeight, 0f, Time.deltaTime * _lerpSpeed);
        }

        _leftArmIK.weight = _leftCurrentWeight;

        // Right arm
        if (Physics.Raycast(_rightRaycastOrigin.position, _rightRaycastOrigin.forward, out RaycastHit scndhit, _maxDistance, _wallMask))
        {
            _rightRigTarget.position = scndhit.point + scndhit.normal * 0.15f;
            //_leftRigTarget.rotation = Quaternion.LookRotation(-hit.normal);

            // Calcule le poids en fonction de la distance
            float t = 1f - Mathf.Clamp01(scndhit.distance / _maxDistance);
            float targetWeight = Mathf.SmoothStep(0f, 1f, t);

            // Interpolation fluide du poids
            _rightCurrentWeight = Mathf.Lerp(_rightCurrentWeight, targetWeight, Time.deltaTime * _lerpSpeed);
        }
        else
        {
            // Loin du mur → retour à 0
            _rightCurrentWeight = Mathf.Lerp(_rightCurrentWeight, 0f, Time.deltaTime * _lerpSpeed);
        }

        _rightArmIK.weight = _rightCurrentWeight;
    }
}
