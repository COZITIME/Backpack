using System;
using UnityEngine;

public class RelicSpawnManager : MonoBehaviour
{
    public static RelicSpawnManager Instance { get; private set; }

    [SerializeField]
    private RandomEntityGetter relicGetter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        XpManager.Instance.OnLevelUp += OnLevelUp;
    }

    private void OnLevelUp(int level)
    {
        var prefab = relicGetter.GetEntity();
        SpawnManager.Instance.Spawn(prefab);
    }

    public void SpawnRelics(Action onComplete = null)
    {
        
    }
}


public enum RelicType
{
    BiteDamage,
    RegurgeDamage,
    BellyUp
}