using System;
using System.Linq;
using Cozi.Random;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    protected RandomWeightedCollection<Transform> spawnPoints;

    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    private void GetSpawnsFromChildren()
    {
        spawnPoints = new RandomWeightedCollection<Transform>(GetComponentsInChildren<Transform>().ToList());
    }

    private Vector2Int GetSpawnPoint()
    {
        Transform spawn = transform;

        try
        {
            spawn = spawnPoints.GetRandomItem
                (sp => MapManager.Instance.IsFree(Vector2Int.RoundToInt(sp.position)));
        }
        catch
        {
            spawn = spawnPoints.GetRandomItem
            (sp =>
            {
                var v2i = Vector2Int.RoundToInt(sp.position);
                var entitesAtSpawn = MapManager.Instance.GetEntitiesAtPosition(v2i);
                if (entitesAtSpawn.All(e => e.EntityData.IsMorsel))
                {
                    entitesAtSpawn.ForEach(e => StartCoroutine(e.EntityData.ForceKill()));
                    return true;
                }

                return false;
            }, true);
        }


        return Vector2Int.RoundToInt(spawn.position);
    }

    public EntityData Spawn(EntityData prefab)
    {
        try
        {
            var sp = SpawnManager.Instance.GetSpawnPoint();

            var newEntity = Instantiate(prefab, (Vector2)sp, Quaternion.identity);

            newEntity.EntityTransform.SnapToPosition(sp);
            TurnManager.Instance.MoveToMapOrder(newEntity.Executor);
            newEntity.Executor.SetStun(1);

            return newEntity;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Out of Spawn Points");
            return null;
        }
    }
}