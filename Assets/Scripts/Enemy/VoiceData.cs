using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoiceData", menuName = "Audio/Voice Data")]
public class VoiceData : ScriptableObject
{
    /// <summary>
    /// Lines with Todd's voice.
    /// </summary>
    [SerializeField]
    private List<AudioClip> _toddLines = new();

    /// <summary>
    /// Lines with Leo's voice.
    /// </summary>
    [SerializeField]
    private List<AudioClip> _leoLines = new();

    /// <summary>
    /// Lines with Jules's voice.
    /// </summary>
    [SerializeField]
    private List<AudioClip> _julesLines = new();

    /// <summary>
    /// The dictionary to hold all voice lines, accessible publicly
    /// </summary>
    public Dictionary<VoiceType, List<AudioClip>> Voices { get; private set; }

    /// <summary>
    /// Called when the ScriptableObject is loaded or enabled.
    /// Use this to initialize the dictionary.
    /// </summary>
    private void OnEnable()
    {
        // Initialize the dictionary here using the private fields.
        Voices = new Dictionary<VoiceType, List<AudioClip>>
        {
            { VoiceType.Todd, _toddLines },
            { VoiceType.Leo, _leoLines },
            { VoiceType.Jules, _julesLines }
        };
    }

    /// <summary>
    /// Called to get a random line of the type we want.
    /// </summary>
    /// <param name="voiceType"> The type of the voice we want. </param>
    public AudioClip GetRandomLine(VoiceType voiceType)
    {
        List<AudioClip> lines = GetLines(voiceType);
        if (lines.Count > 0)
        {
            return lines[Random.Range(0, lines.Count)];
        }
        Debug.LogError("No line for " + name + " with the type " + voiceType);
        return null;
    }

    /// <summary>
    /// Called to get the list with the type we want.
    /// </summary>
    /// <param name="voiceType"> The type of the voice we want. </param>
    /// <returns></returns>
    private List<AudioClip> GetLines(VoiceType voiceType)
    {
        if (Voices.TryGetValue(voiceType, out List<AudioClip> lines))
        {
            return lines;
        }
        return new();
    }
}
