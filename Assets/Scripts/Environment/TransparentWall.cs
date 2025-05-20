using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] LayerMask _wallLayerMask;

    Material _wallMat;

    private void Update()
    {
        if (Physics.Raycast(transform.position, _player.position - transform.position, out RaycastHit hit, 100f, _wallLayerMask))
        {
            _wallMat = hit.transform.GetComponent<Renderer>().material;
            _wallMat.SetFloat("_Opacity", 0.5f);
        }
        else
        {
            if (_wallMat != null)
            {
                _wallMat.SetFloat("_Opacity", 1f);
            }
        }
    }
}
