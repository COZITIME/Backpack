using System;
using UnityEngine;

public class MorselSpawningManager : MonoBehaviour
{
    public static MorselSpawningManager Instance { get; private set; }

    [SerializeField]
    private EntityTransform morselPrefab;

    [SerializeField]
    private EntityTransform morselCookedPrefab;

    [SerializeField]
    private EntityTransform heartPrefab;

    [SerializeField]
    private EntityTransform heartCookedPrefab;
    
    private void Awake()
    {
        Instance = this;
    }

    public void OnSpawnFromEntity(EntityTransform oldEntity, bool isCooked = false, bool isHeart = false)
    {
        var prefab = isHeart
            ? (isCooked ? heartCookedPrefab : heartPrefab)
            : (isCooked ? morselCookedPrefab : morselPrefab);

        var spawn = Instantiate(prefab, oldEntity.transform.position, Quaternion.identity);

        int? bellyIndex = oldEntity.BellyIndex;
        if (bellyIndex.HasValue)
        {
            BellyManager.Instance.PlayerBelly.ReplaceAtIndex(bellyIndex.Value, spawn, oldEntity);
        }
        else
        {
            spawn.SnapToPosition(Vector2Int.RoundToInt(oldEntity.transform.position));
        }

        TurnManager.Instance.Replace(bellyIndex.HasValue, spawn.EntityExecutor, oldEntity.EntityExecutor);
    }
}