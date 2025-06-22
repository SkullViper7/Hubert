using System.Collections;
using UnityEngine;

public class EnemyDissolve : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer _meshRenderer;
    Material _enemyMaterial;

    [SerializeField] GameObject _enemy;

    private void Start()
    {
        _enemyMaterial = _meshRenderer.material;
    }

    public void Dissolve()
    {
        StartCoroutine(LerpTransparencyOverTime(0f, 1f, 1f));
    }

    IEnumerator LerpTransparencyOverTime(float start, float end, float duration)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            _enemyMaterial.SetFloat("_Transparency", Mathf.Lerp(start, end, timeElapsed / duration));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(_enemy);
    }
}
