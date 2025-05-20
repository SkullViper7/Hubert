using UnityEngine;

public class FallSmoke : MonoBehaviour
{
    [SerializeField] GameObject _fallSmoke;
    bool _isInstanciated;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!_isInstanciated)
            {
                _fallSmoke.transform.position = collision.contacts[0].point;
                _fallSmoke.SetActive(true);
                _isInstanciated = true;
            }
        }
    }
}
