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
        if (body == null) return;

        if (body.gameObject.layer == LayerMask.NameToLayer("Breakable") || body.gameObject.layer == LayerMask.NameToLayer("PushableObject"))
        {
            body.isKinematic = false;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = body.transform.position - transform.position;
            pushDir.y = 0;

            // Apply the push and take strength into account
            body.AddForceAtPosition(Mathf.Max(_characterController.velocity.magnitude, 1f) * _strength * pushDir, hit.point, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // make sure we hit a non kinematic rigidbody
        Rigidbody body = collision.collider.attachedRigidbody;
        if (body == null) return;

        if (body.gameObject.layer == LayerMask.NameToLayer("Breakable") || body.gameObject.layer == LayerMask.NameToLayer("PushableObject"))
        {
            body.isKinematic = false;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = body.transform.position - transform.position;
            pushDir.y = 0;

            // Apply the push and take strength into account
            body.AddForceAtPosition(Mathf.Max(_characterController.velocity.magnitude, 1f) * _strength * pushDir, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
}