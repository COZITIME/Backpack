using System;
using System.Collections;
using System.Linq;
using Cozi.Random;
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

    [SerializeField]
    protected RandomWeightedCollection<Transform> spawnPoints;

    private int turnCountDown = 0;

    private float lastSpawnProgression = 0;

    [ShowInInspector, DisplayAsString]
    private float _nextWait;

    private void Awake()
    {
        Instance = this;
        _nextWait = GetWait(0f);
    }

    [Button]
    private void GetSpawnsFromChildren()
    {
        spawnPoints = new RandomWeightedCollection<Transform>(GetComponentsInChildren<Transform>().ToList());
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
        var sp = GetSpawnPoint();
        var prefab = getter.GetEntity();
        var newEntity = Instantiate(prefab, (Vector2)sp, Quaternion.identity);
        
        newEntity.EntityTransform.SnapToPosition(sp);
        TurnManager.Instance.MoveToMapOrder(newEntity.Executor);
    }


    private Vector2Int GetSpawnPoint()
    {
        var spawn = spawnPoints.GetRandomItem
            (sp => MapManager.Instance.IsFree(Vector2Int.RoundToInt(sp.position)));

        return Vector2Int.RoundToInt(spawn.position);
    }
}