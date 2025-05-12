using UnityEngine;

public class HubertVFXManager : MonoBehaviour
{
    [Header("Footsteps")]
    [SerializeField] GameObject _footstepVFX;
    [SerializeField] Transform _leftFoot;
    [SerializeField] Transform _rightFoot;

    [Header("Electified")]
    [SerializeField] GameObject _smokeVFX;
    [SerializeField] GameObject _electifiedMesh;
    [SerializeField] GameObject _fallSmoke;

    public void PlayLeftFootstep()
    {
        GameObject newVFX = Instantiate(_footstepVFX, _leftFoot.position, Quaternion.identity);
        Destroy(newVFX, 0.75f);
    }

    public void PlayRightFootstep()
    {
        GameObject newVFX = Instantiate(_footstepVFX, _rightFoot.position, Quaternion.identity);
        Destroy(newVFX, 0.75f);
    }

    public void PlayElectifiedSmoke()
    {
        _smokeVFX.SetActive(true);
    }

    public void EnableElectifiedMesh()
    {
        _electifiedMesh.SetActive(true);
    }

    public void DisableElectifiedMesh()
    {
        _electifiedMesh.SetActive(false);
    }

    public void PlayFallSmoke()
    {
        _fallSmoke.SetActive(true);
    }

    [System.Obsolete]
    public void CamShake()
    {
        ImpulseManager.Instance.Impulse();
    }
}
