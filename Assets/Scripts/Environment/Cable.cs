using UnityEngine;

public class Cable : MonoBehaviour
{
    Material _cableMaterial;
    [SerializeField] float _revealDistance;

    Transform _playerPos;

    void Start()
    {
        _playerPos = GameObject.Find("Player").transform;
        _cableMaterial = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, _playerPos.position);
        _cableMaterial.SetFloat("_RevealFactor", Mathf.InverseLerp(_revealDistance, 0f, distance));
    }
}
