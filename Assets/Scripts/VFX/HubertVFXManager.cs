using UnityEngine;

public class HubertVFXManager : MonoBehaviour
{
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
}
