using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField]
    private AudioSource regularMusic;

    [SerializeField]
    private AudioSource oneLifeMusic;

    private AudioSource _currentAudioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SuperStart();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SuperStart();
    }

    private void SuperStart()
    {
        this.ExecuteDelayedFrame(1, () =>
        {
            var instanceEntityData = PlayerTransform.Instance.EntityData;
            instanceEntityData.OnHealthChange += OnHealthChanged;
            OnHealthChanged(instanceEntityData.Health);
        });
    }

    private void OnHealthChanged(int hp)
    {
        var audioSource = (hp <= 1) ? oneLifeMusic : regularMusic;
        if (audioSource == _currentAudioSource) return;

        float progress = 0f;
        if (_currentAudioSource)
        {
            _currentAudioSource.DOFade(0f, .5f);
        }

        _currentAudioSource = audioSource;
        _currentAudioSource.DOFade(1f, .5f);
        _currentAudioSource.Play();
        _currentAudioSource.time = progress;
    }
}