using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] List<string> _dialogues;

    [SerializeField] GlitchText _glitchText;

    private void Start()
    {
        StartDialogue(0);
    }

    public void StartDialogue(int index)
    {
        StartCoroutine(_glitchText.GlitchReveal(_dialogues[index]));
    }
}
