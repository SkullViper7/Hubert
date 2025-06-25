using UnityEngine;
using UnityEngine.VFX;

public class HubertVFX : MonoBehaviour
{
    [SerializeField, Header("Footsteps")] 
    private GameObject _footstepVFX;

    [SerializeField] 
    private Transform _leftFoot;

    [SerializeField]
    private Transform _rightFoot;

    [SerializeField, Header("Electified")] 
    private GameObject _headSmokeVFX;

    [SerializeField] 
    private GameObject _bodySmokeVFX;

    [SerializeField] 
    private GameObject _electifiedMesh;

    [SerializeField] 
    private GameObject _mesh;

    [SerializeField] 
    private Material _electrifiedMaterial;

    [SerializeField] 
    private Material _standardMaterial;

    [SerializeField] 
    private GameObject _crossEyes;

    [SerializeField] 
    private Material _burntMaterial;

    [SerializeField] 
    private GameObject _skeleton;

    [SerializeField]
    private GameObject _fallSmoke;

    [SerializeField, Space, Header("Hit")]
    private GameObject _hitVFX;

    [SerializeField, Space, Header("Shoot")] 
    private GameObject _trunkSmokeVFX;

    [SerializeField]
    private GameObject _shootVFX;

    [SerializeField, Space, Header("Cables")] 
    private Material _cableOffMaterial;

    [SerializeField] 
    private Material _cableOnMaterial;

    /// <summary>
    /// Player state manager.
    /// </summary>
    [Space, SerializeField]
    private PlayerStateManager _playerStateManager;

    /// <summary>
    /// Animation controller of hubert.
    /// </summary>
    private PlayerAnimationController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerAnimationController>();
    }

    private void Start()
    {
        _controller.OnLeftStep += PlayLeftFootstep;
        _controller.OnRightStep += PlayRightFootstep;
        _controller.MustShoot += PlayShootVFX;
        _controller.OnShot += ShowTrunkSmoke;
        _playerStateManager.OnShootCooldownEnded += HideTrunkSmoke;
        _controller.MustHit += PlayHitVFX;
        _controller.OnStartElectrified += StartElectrified;
        _controller.OnEndElectrified += EndElectrified;
        _controller.OnFall += PlayFallSmoke;
    }

    private void Update()
    {
        _cableOffMaterial.SetVector("_3DPlayerPos", transform.position);
        _cableOnMaterial.SetVector("_3DPlayerPos", transform.position);
    }

    private void OnDisable()
    {
        _cableOffMaterial.SetVector("_3DPlayerPos", Vector3.zero);
        _cableOnMaterial.SetVector("_3DPlayerPos", Vector3.zero);
    }

    private void PlayLeftFootstep()
    {
        GameObject newVFX = Instantiate(_footstepVFX, _leftFoot.position, Quaternion.identity);
        Destroy(newVFX, 0.75f);
    }

    private void PlayRightFootstep()
    {
        GameObject newVFX = Instantiate(_footstepVFX, _rightFoot.position, Quaternion.identity);
        Destroy(newVFX, 0.75f);
    }

    private void PlayShootVFX()
    {
        _shootVFX.SetActive(true);
        Invoke(nameof(DisableShootVFX), 0.5f);
    }

    private void DisableShootVFX()
    {
        _shootVFX.SetActive(false);
    }

    private void ShowTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(true);
        _trunkSmokeVFX.GetComponent<VisualEffect>().SetFloat("StartTime", Time.time);
    }

    private void HideTrunkSmoke()
    {
        _trunkSmokeVFX.SetActive(false);
    }

    private void PlayHitVFX()
    {
        _hitVFX.SetActive(true);
        Invoke(nameof(DisableHitVFX), 0.5f);
    }

    private void DisableHitVFX()
    {
        _hitVFX.SetActive(false);
    }

    private void StartElectrified()
    {
        _headSmokeVFX.SetActive(true);
        _bodySmokeVFX.SetActive(true);
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _electrifiedMaterial;
        _skeleton.SetActive(true);
        _electifiedMesh.SetActive(true);
        _crossEyes.SetActive(true);
    }

    private void EndElectrified()
    {
        _electifiedMesh.SetActive(false);
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _standardMaterial;
        _skeleton.SetActive(false);
        _mesh.GetComponent<SkinnedMeshRenderer>().material = _burntMaterial;
    }

    private void PlayFallSmoke()
    {
        _fallSmoke.SetActive(true);
    }
}
