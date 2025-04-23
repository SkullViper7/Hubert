using System.Collections;
using TMPro;
using UnityEngine;

public class GlitchText : MonoBehaviour
{
    TMP_Text _textDisplay;
    [SerializeField] float _totalRevealTime = 2f;
    [SerializeField] float _scrambleSpeed = 0.05f;
    char[] _displayChars;
    string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

    private void Start()
    {
        _textDisplay = GetComponent<TMP_Text>();
    }

    public IEnumerator GlitchReveal(string textToReveal)
    {
        _displayChars = new char[textToReveal.Length];
        for (int i = 0; i < _displayChars.Length; i++)
        {
            _displayChars[i] = _chars[Random.Range(0, _chars.Length)];
        }

        float elapsed = 0f;
        float[] lockTimes = new float[textToReveal.Length];
        for (int i = 0; i < lockTimes.Length; i++)
        {
            lockTimes[i] = Random.Range(0f, _totalRevealTime);
        }

        while (elapsed < _totalRevealTime)
        {
            elapsed += _scrambleSpeed;

            for (int i = 0; i < _displayChars.Length; i++)
            {
                if (elapsed >= lockTimes[i])
                {
                    _displayChars[i] = textToReveal[i];
                }
                else
                {
                    _displayChars[i] = _chars[Random.Range(0, _chars.Length)];
                }
            }

            _textDisplay.text = new string(_displayChars);
            yield return new WaitForSeconds(_scrambleSpeed);
        }

        _textDisplay.text = textToReveal;
    }
}
