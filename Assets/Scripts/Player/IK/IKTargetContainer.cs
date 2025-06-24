using System.Collections.Generic;
using UnityEngine;

public class IKTargetContainer : MonoBehaviour
{
    [SerializeField] List<Transform> _targets;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IKTargetManager.Instance.SwitchTargets(_targets);
        }
    }
}
