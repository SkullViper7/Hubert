using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCam : MonoBehaviour
{
    Animator _animator;
    [SerializeField] AnimationClip _zoom;

    PlayerStateManager _player;

    /// <summary>
    /// Called to init all listeners when the player is instanciated.
    /// </summary>
    void Awake()
    {
        _animator = GetComponent<Animator>();

        // When the player is instanciated, store its reference and init all listeners.
        GameManager.Instance.OnPlayerInstanciated += player =>
        {
            _player = player;
            _player.OnFall += Zoom;
        };
    }

    void Zoom()
    {
        _animator.enabled = true;
        _animator.Play(_zoom.name);
        StartCoroutine(DeathClose());
    }

    IEnumerator DeathClose()
    {
        yield return new WaitForSeconds(_zoom.length);

        LooneyTunesManager.Instance.PlayDeathCloseAnim();

        yield return new WaitForSeconds(LooneyTunesManager.Instance.DeathClose.length);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
