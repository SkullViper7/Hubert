using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	[SerializeField, Range(0.1f, 5f)]
    private float _strength;

	private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
	{
        // make sure we hit a non kinematic rigidbody
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f) return;

        // Calculate push direction from move direction, horizontal motion only
        Vector3 pushDir = new (hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        // Apply the push and take strength into account
        body.AddForceAtPosition(pushDir * _characterController.velocity.magnitude * _strength, hit.point, ForceMode.Impulse);
    }
}