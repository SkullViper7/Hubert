using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKTargetManager : MonoBehaviour
{
    public static IKTargetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    WeightedTransformArray _weightedArray = new WeightedTransformArray();
    [SerializeField] float _blendSpeed = 2f;
    [SerializeField] float _maxDistance = 10f;
    [SerializeField] float _clearBlendSpeed = 1f;
    [SerializeField] float _maxViewAngle = 60f;

    int _currentIndex = -1;

    [SerializeField] List<Transform> _targets;

    [SerializeField] MultiAimConstraint _multiAimConstraint;
    [SerializeField] RigBuilder _rigBuilder;
    [SerializeField] Transform _player;

    void FixedUpdate()
    {
        UpdateClosestTarget();
        SmoothBlendTargets();
    }

    public void SwitchTargets(List<Transform> targets)
    {
        _targets.Clear();
        _targets = targets;
    }

    /// <summary>
    /// Updates the closest target to the player.
    /// </summary>
    void UpdateClosestTarget()
    {
        // If there are no targets, return
        if (_targets == null || _targets.Count == 0) return;

        // Find the closest target to the player
        float shortestDistance = float.MaxValue;
        Transform closestTarget = null;

        // Iterate through all targets
        foreach (var target in _targets)
        {
            // Calculate the distance between the player and the target
            float distance = Vector3.Distance(_player.position, target.position);

            // If the target is closer than the current closest target and within the max distance, update the closest target
            if (distance < shortestDistance && distance <= _maxDistance)
            {
                shortestDistance = distance;
                closestTarget = target;
            }
        }

        // If a closest target was found, set the target smoothly
        if (closestTarget != null)
        {
            SetTargetSmoothly(closestTarget);
        }
        // Otherwise, clear the targets smoothly
        else
        {
            _currentIndex = -1;
            ClearTargetsSmoothly();
        }
    }

    /// <summary>
    /// Smoothly blends the weights of the IK targets.
    /// </summary>
    void SmoothBlendTargets()
    {
        for (int i = 0; i < _weightedArray.Count; i++)
        {
            float currentWeight = _weightedArray[i].weight;
            float targetWeight = 0f;

            if (i == _currentIndex)
            {
                Vector3 toTarget = (_weightedArray[i].transform.position - _player.position).normalized;
                float angle = Vector3.Angle(_player.forward, toTarget);

                // Si l'angle est inférieur au max autorisé, on vise cette cible
                if (angle <= _maxViewAngle)
                {
                    targetWeight = 1f;
                }
                else
                {
                    targetWeight = 0f;
                }
            }

            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * _blendSpeed);
            _weightedArray.SetWeight(i, newWeight);
        }

        var data = _multiAimConstraint.data;
        data.sourceObjects = _weightedArray;
        _multiAimConstraint.data = data;
        _rigBuilder.Build();
    }


    /// <summary>
    /// Smoothly blends the weights of the IK targets to the given target.
    /// </summary>
    /// <param name="newTarget">The target to set the weights to.</param>
    void SetTargetSmoothly(Transform newTarget)
    {
        int index = -1;

        // Iterate through all the targets
        for (int i = 0; i < _weightedArray.Count; i++)
        {
            // Check if the target is already in the array
            if (_weightedArray[i].transform == newTarget)
            {
                index = i;
                break;
            }
        }

        // If the target is not in the array, add it
        if (index == -1)
        {
            _weightedArray.Add(new WeightedTransform(newTarget, 0f));
            index = _weightedArray.Count - 1;
        }

        // Set the current index to the target
        _currentIndex = index;
    }

    /// <summary>
    /// Smoothly blends the weights of the IK targets to zero when no target is close enough.
    /// </summary>
    void ClearTargetsSmoothly()
    {
        bool anyTargetClose = false;

        // Check if any target is close to the player
        foreach (var target in _targets)
        {
            if (Vector3.Distance(_player.position, target.position) <= _maxDistance)
            {
                anyTargetClose = true;
                break;
            }
        }

        // If no target is close, clear the targets smoothly
        if (!anyTargetClose && _currentIndex != -1)
        {
            WeightedTransform lastTarget = _weightedArray[_currentIndex];
            // Move the weight of the last target towards zero
            lastTarget.weight = Mathf.MoveTowards(lastTarget.weight, 0f, Time.deltaTime * _clearBlendSpeed);

            // Update the weight of the last target
            _weightedArray.SetWeight(_currentIndex, lastTarget.weight);

            // If the last target is cleared, clear the array and reset the current index
            if (lastTarget.weight == 0f)
            {
                _weightedArray.Clear();
                _currentIndex = -1;
            }
        }

        // Update the constraint with the new weights
        var data = _multiAimConstraint.data;
        data.sourceObjects = _weightedArray;
        _multiAimConstraint.data = data;

        // Build the rig again with the new weights
        _rigBuilder.Build();
    }
}
