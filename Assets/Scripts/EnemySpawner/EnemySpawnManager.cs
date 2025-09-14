using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }

    [SerializeField]
    private RandomEntityGetter getter;

    [SerializeField]
    private int startingEnemies = 2;

    [SerializeField]
    private AnimationCurve durationToSpawnOverProgressionCurve =
        new AnimationCurve(new Keyframe(0, 4), new Keyframe(200, 0.8f));


    private int turnCountDown = 0;

    private float lastSpawnProgression = 0;

    [ShowInInspector, DisplayAsString]
    private float _nextWait;

    private void Awake()
    {
        Instance = this;
        _nextWait = GetWait(0f);
    }


    private float GetWait(float t) => durationToSpawnOverProgressionCurve.Evaluate(t);


    private void Start()
    {
        for (int i = 0; i < startingEnemies; i++)
        {
            Spawn();
        }

        ProgressionManager.Instance.OnProgressionChanged += OnProgressionChanged;
        OnProgressionChanged(0f);
    }

    private void OnProgressionChanged(float progression)
    {
        for (int i = 0; i < 10; i++)
        {
            if (progression < _nextWait) return;
            float summedProgression = _nextWait - progression;
            _nextWait += GetWait(summedProgression);
            Spawn();
        }
    }

    public void Spawn()
    {
        var prefab = getter.GetEntity();
        SpawnManager.Instance.Spawn(prefab);
    }
}