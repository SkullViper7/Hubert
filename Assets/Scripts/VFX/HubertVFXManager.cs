using UnityEngine;
using UnityEngine.VFX;

public class HubertVFXManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager _playerStateManager;

    [Header("Footsteps")]
    [SerializeField] GameObject _footstepVFX;
    [SerializeField] Transform _leftFoot;
    [SerializeField] Transform _rightFoot;

    [Header("Electified")]
    [SerializeField] GameObject _headSmokeVFX;
    [SerializeField] GameObject _bodySmokeVFX;
    [SerializeField] GameObject _electifiedMesh;
    [SerializeField] GameObject _mesh;
    [SerializeField] Material _electrifiedMaterial;
    [SerializeField] Material _standardMaterial;
    [SerializeField] GameObject _crossEyes;
    [SerializeField] Material _burntMaterial;
    [SerializeField] GameObject _skeleton;
    [SerializeField] GameObject _fallSmoke;

    [Header("Shoot")]
    [SerializeField] GameObject _trunkSmokeVFX;

    private void Start()
    {
        _playerStateManager.AimingState.OnShoot += ShowTrunkSmoke;
        _playerStateManager.OnShootCooldownEnded += HideTrunkSmoke;
    }

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
        _headSmokeVFX.SetActive(true);
        _bodySmokeVFX.SetActive(true);
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

    public void SetElectrifiedMaterial()
    {
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _electrifiedMaterial;
        _skeleton.SetActive(true);
    }

    public void DisableElectrifiedMaterial()
    {
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _standardMaterial;
        _skeleton.SetActive(false);
    }

    public void SetCrossEyes()
    {
        _crossEyes.SetActive(true);
    }

    public void SetBurntMaterial()
    {
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _burntMaterial;
    }

    public void ShowTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(true);
    }

    public void HideTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(false);
        _trunkSmokeVFX.GetComponent<VisualEffect>().SetFloat("StartTime", Time.time);
    }
}
