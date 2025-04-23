using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public List<string> Dialogues;

    [SerializeField] GlitchText _glitchText;

    private void Start()
    {
        StartDialogue(0);
    }

    public void StartDialogue(int index)
    {
        StartCoroutine(_glitchText.GlitchReveal(Dialogues[index]));
    }
}
