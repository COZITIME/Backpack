using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Belly
{
    public event Action<List<EntityTransform>> OnBellyRepositioning;

    private readonly List<EntityTransform> _entitiesInBelly = new List<EntityTransform>();
    private readonly int _size = 10;
    private readonly float _spacing = 1f;
    private readonly Transform _bellyTop;
    private int SlotsLeft => _size - _entitiesInBelly.Count;

    public Belly(int size, float spacing, Transform bellyTop)
    {
        _size = size;
        _spacing = spacing;
        _bellyTop = bellyTop;
    }

    public List<EntityTransform> GetEntitiesWithin => _entitiesInBelly;


    public bool TryEat(EntityTransform entityTransform)
    {
        if (!entityTransform) return false;
        if (!entityTransform.IsEaten && _entitiesInBelly.Count >= _size) return false;

        _entitiesInBelly.Add(entityTransform);

        TurnManager.Instance.MoveToBellyOrder(entityTransform.GetComponent<EntityExecutor>());
        Reposition();
        return true;
    }

    public bool HasRoomForEntities(int amount)
    {
        return (_entitiesInBelly.Count + amount) <= _size;
    }

    public bool TryRegurgitate(Vector2Int from, Vector2Int position, FaceDirection direction,
        out EntityTransform entityTransform)
    {
        entityTransform = _entitiesInBelly.LastOrDefault();
        if (!entityTransform) return false;
        _entitiesInBelly.Remove(entityTransform);

        entityTransform.Regurgitate(from, position, direction);
        entityTransform.FaceInDirection(direction);

        TurnManager.Instance.MoveToMapOrder(entityTransform.GetComponent<EntityExecutor>());
        Reposition();

        return true;
    }

    private void Reposition()
    {
        var count = _entitiesInBelly.Count;
        for (var index = 0; index < count; index++)
        {
            var entity = _entitiesInBelly[index];
            int posIndex = (count - index) - 1;
            Vector2 position = (Vector2)_bellyTop.position + (Vector2.down * (_spacing * posIndex));
            entity.SetBellyPosition(index, position);
        }

        OnBellyRepositioning?.Invoke(_entitiesInBelly);
    }

    public void Remove(EntityTransform entityTransform)
    {
        _entitiesInBelly.Remove(entityTransform);
        Reposition();
    }
}