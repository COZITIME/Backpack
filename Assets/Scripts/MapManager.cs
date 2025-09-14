using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    private HashSet<EntityTransform> _entities = new HashSet<EntityTransform>();

    [FormerlySerializedAs("collisionMap")] [SerializeField]
    private Tilemap wallMap;


    private void Awake()
    {
        Instance = this;
    }

    public HashSet<EntityTransform> Entities => _entities;

    public void AddEntity(EntityTransform entityTransform)
    {
        if (_entities.Contains(entityTransform)) return;
        _entities.Add(entityTransform);
    }

    public void RemoveEntity(EntityTransform entityTransform)
    {
        if (!_entities.Contains(entityTransform)) return;
        _entities.Remove(entityTransform);
    }

    public List<EntityTransform> GetEntitiesAtPosition(Vector2Int position)
    {
        return _entities.Where(e => !e.IsEaten && e.MapPosition == position).ToList();
    }

    public bool IsWallAtPosition(Vector2Int position)
    {
        var gridPos = (Vector3Int)position;
        return wallMap.GetTile(gridPos);
    }

    public bool IsFree(Vector2Int position, bool avoidWalls = true, bool avoidEntities = true)
    {
        if (avoidWalls)
        {
            if (IsWallAtPosition(position)) return false;
        }

        if (avoidEntities)
        {
            if (GetEntitiesAtPosition(position).Count > 0) return false;
        }

        return true;
    }
}