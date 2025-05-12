using Cinemachine;
using UnityEngine;

public class DynamicCam : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _dollyCam;
    [SerializeField] CinemachineFreeLook _playerCam;
    [SerializeField] CinemachineDollyCart _cart;

    [SerializeField] Transform _player;
    [SerializeField] Transform _endTarget;
    [SerializeField] Transform _startTarget;

    public void SetDollyCam()
    {
        _dollyCam.Priority = 10;
        _playerCam.Priority = 0;
    }

    public void SetFreeCam()
    {
        _dollyCam.Priority = 0;
        _playerCam.Priority = 10;
    }

    void Update()
    {
        FollowCart();
    }

    void FollowCart()
    {
        if (_dollyCam.Priority == 10)
        {
            float totalDistance = Vector3.Distance(_startTarget.position, _endTarget.position);
            float currentDistance = Vector3.Distance(_player.position, _endTarget.position);

            float t = 1f - Mathf.Clamp01(currentDistance / totalDistance);
            _cart.m_Position = t * _cart.m_Path.PathLength;
        }
    }
}
