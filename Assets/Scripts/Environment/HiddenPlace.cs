using UnityEngine;

public class HiddenPlace : MonoBehaviour
{
    /// <summary>
    /// Position where player goes to hide.
    /// </summary>
    [SerializeField, Header("Entry")]
    private Vector3 _hidingPosition;

    /// <summary>
    /// Rotation where player look to hide.
    /// </summary>
    [SerializeField]
    private Vector3 _hidingRotation;

    /// <summary>
    /// Position where player goes to exit the hidden place.
    /// </summary>
    [SerializeField, Space, Header("Exit")]
    private Vector3 _exitPosition;

    /// <summary>
    /// Rotation where player look to exit.
    /// </summary>
    [SerializeField]
    private Vector3 _exitRotation;

    /// <summary>
    /// Get the position where player goes to hide.
    /// </summary>
    public Vector3 HidingPosition { get { return transform.TransformPoint(_hidingPosition); } private set { } }

    /// <summary>
    /// Get the rotation where player look to hide.
    /// </summary>
    public Quaternion HidingRotation { get { return Quaternion.LookRotation(_hidingRotation.normalized); } private set { } }

    /// <summary>
    /// Get the position where player goes to exit the hidden place.
    /// </summary>
    public Vector3 ExitPosition { get { return transform.TransformPoint(_exitPosition); } private set { } }

    /// <summary>
    /// Get the rotation where player look to exit.
    /// </summary>
    public Quaternion ExitRotation { get { return Quaternion.LookRotation(_exitRotation.normalized); } private set { } }

    /// <summary>
    /// The animation that the player has to do when he is hidden to this place.
    /// </summary>
    [field: SerializeField]
    public string PlayerAnimation { get; private set; }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(_hidingPosition), 0.1f);
        Gizmos.DrawLine(HidingPosition, HidingPosition + _hidingRotation.normalized * 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(_exitPosition), 0.1f);
        Gizmos.DrawLine(ExitPosition, ExitPosition + _exitRotation.normalized * 0.2f);
    }
#endif
}
