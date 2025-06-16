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

    [Header("Hit")]
    [SerializeField] GameObject _hitVFX;

    [Header("Shoot")]
    [SerializeField] GameObject _trunkSmokeVFX;
    [SerializeField] GameObject _shootVFX;

    [Header("Cables")]
    [SerializeField] Material _cableOffMaterial;
    [SerializeField] Material _cableOnMaterial;

    private void Start()
    {
        _playerStateManager.AimingState.OnShoot += ShowTrunkSmoke;
        _playerStateManager.OnShootCooldownEnded += HideTrunkSmoke;
    }

    void Update()
    {
        _cableOffMaterial.SetVector("_3DPlayerPos", transform.position);
        _cableOnMaterial.SetVector("_3DPlayerPos", transform.position);
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

    public void PlayHitVFX()
    {
        _hitVFX.SetActive(true);
        Invoke(nameof(DisableHitVFX), 0.5f);
    }

    private void DisableHitVFX()
    {
        _hitVFX.SetActive(false);
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

    public void PlayShootVFX()
    {
        _shootVFX.SetActive(true);
        Invoke(nameof(DisableShootVFX), 0.5f);
    }

    void DisableShootVFX()
    {
        _shootVFX.SetActive(false);
    }

    public void ShowTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(true);
        _trunkSmokeVFX.GetComponent<VisualEffect>().SetFloat("StartTime", Time.time);
    }

    public void HideTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(false);
    }
}
