using System.Collections.Generic;
using UnityEngine;

public class ResetMat : MonoBehaviour
{
    [SerializeField] Material _redTrunk;
    [SerializeField] Material _redHead;
    [SerializeField] List<Material> _wallMats;
    [SerializeField] Material _cableMat;

    void OnDisable()
    {
        _redHead.SetFloat("_Height", 0f);
        _redTrunk.SetFloat("_Height", 0f);
        _cableMat.SetFloat("_ElipseSize", 1f);

        for (int i = 0; i < _wallMats.Count; i++)
        {
            _wallMats[i].SetFloat("_ElipseSize", 1f);
        }
    }
}
