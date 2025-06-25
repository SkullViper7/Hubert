using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCam : MonoBehaviour
{
    PlayerStateManager _player;
    float _currentCamZoom;

    /// <summary>
    /// Called to init all listeners when the player is instanciated.
    /// </summary>
    void Awake()
    {
        // When the player is instanciated, store its reference and init all listeners.
        GameManager.Instance.OnPlayerInstanciated += player =>
        {
            _player = player;
            _player.OnFall += Zoom;
        };
    }

    void Zoom()
    {
        StartCoroutine(LerpDeathCam());
    }

    IEnumerator LerpDeathCam()
    {
        _currentCamZoom = _player.Camera.m_YAxis.Value;

        float timer = 0f;

        while (timer < 1f)
        {
            _player.Camera.m_YAxis.Value = Mathf.Lerp(_currentCamZoom, 0f, timer);

            timer += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(DeathClose());
    }

    IEnumerator DeathClose()
    {
        LooneyTunesManager.Instance.PlayDeathCloseAnim();

        yield return new WaitForSeconds(LooneyTunesManager.Instance.DeathClose.length);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
