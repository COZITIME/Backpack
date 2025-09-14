using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Belly
{
    public event Action<List<EntityTransform>> OnBellyRepositioning;

    private readonly List<EntityTransform> _entitiesInBelly = new List<EntityTransform>();
    private readonly int _startSize = 10;
    private readonly float _spacing = 1f;
    private readonly Transform _bellyTop;
    private int _bonusBellySize = 0;
    private int SlotsLeft => BellySize - _entitiesInBelly.Count;

    public int BellySize => _startSize + _bonusBellySize;

    public Belly(int startSize, float spacing, Transform bellyTop)
    {
        _startSize = startSize;
        _spacing = spacing;
        _bellyTop = bellyTop;
    }

    public List<EntityTransform> GetEntitiesWithin => _entitiesInBelly;
    public bool IsFull => _entitiesInBelly.Count >= BellySize;


    public bool TryEat(EntityTransform entityTransform)
    {
        if (!entityTransform) return false;
        if (!entityTransform.IsEaten && _entitiesInBelly.Count >= BellySize) return false;

        _entitiesInBelly.Add(entityTransform);

        TurnManager.Instance.MoveToBellyOrder(entityTransform.GetComponent<EntityExecutor>());
        Reposition();

        if (entityTransform.EntityData.IsRelic)
        {
            RelicManager.Instance.Eat(entityTransform.EntityData.RelicType);
        }

        var executor = entityTransform.EntityExecutor;
        executor.StartCoroutine(executor.OnEatenCoroutine());

        return true;
    }

    public bool HasRoomForEntities(int amount)
    {
        return (_entitiesInBelly.Count + amount) <= BellySize;
    }

    public bool TryRegurgitate(Vector2Int from, Vector2Int position, FaceDirection direction,
        out EntityTransform entityTransform)
    {
        entityTransform = _entitiesInBelly.LastOrDefault();
        if (!entityTransform) return false;
        _entitiesInBelly.Remove(entityTransform);

        entityTransform.Regurgitate(from, position, direction);
        entityTransform.FaceInDirection(direction);

        var executor = entityTransform.EntityExecutor;
        TurnManager.Instance.MoveToMapOrder(executor);
        executor.SetStun(1);
        Reposition();

        if (entityTransform.EntityData.IsRelic)
        {
            RelicManager.Instance.Regurgitate(entityTransform.EntityData.RelicType);
        }

        var vomit = ParticleManager.Instance.PlayParticles(ParticleType.Vomit, position);
        vomit.transform.up = (Vector2)(position - from);
        // vomit.transform.DOMove((Vector2)position, .5f);

        executor.StartCoroutine(executor.OnRegurgitatedCoroutine());


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

    public void ReplaceAtIndex(int index, EntityTransform newEntity, EntityTransform old)
    {
        if (index > _entitiesInBelly.Count || _entitiesInBelly[index] != old)
        {
            _entitiesInBelly.Insert(index, newEntity);
        }
        else
        {
            _entitiesInBelly[index] = newEntity;
        }

        Reposition();
    }

    public void SetBellyBonus(int relicCount)
    {
        _bonusBellySize = relicCount * 2;
        Reposition();
    }
}