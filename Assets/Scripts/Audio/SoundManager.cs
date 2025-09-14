using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private AudioSource[] audioSource;

    [SerializeField]
    private SerializedDictionary<SoundType, SoundData> sounds;

    private int _sourceIndex = 0;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnValidate()
    {
        if (audioSource == null || audioSource.Length == 0)
        {
            audioSource = GetComponentsInChildren<AudioSource>();
        }
    }

    public void Play(SoundType sound)
    {
        if (sounds.TryGetValue(sound, out var clip))
        {
            audioSource[_sourceIndex].pitch = Random.Range(0.8f, 1.2f);
            audioSource[_sourceIndex].PlayOneShot(clip.AudioClip);

            _sourceIndex++;
            _sourceIndex = _sourceIndex % audioSource.Length;
        }
    }
}