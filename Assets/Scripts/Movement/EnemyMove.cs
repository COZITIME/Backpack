using System;
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public EntityTransform EntityTransform { get; private set; }
    public EntityData EntityData { get; private set; }

    private void Awake()
    {
        EntityTransform = GetComponent<EntityTransform>();
        EntityData = GetComponent<EntityData>();
    }

    public float GetDistanceFromPlayer()
    {
        var dist = Vector2Int.Distance(EntityTransform.MapPosition, PlayerTransform.Instance.MapPosition);
        return dist;
    }

    public IEnumerator TryMoveToPlayer()
    {
        if (Pathfinder.TryGetFirstStep(EntityTransform, PlayerTransform.Instance, out var step))
        {
            if (EntityTransform.TryMoveTo(step))
            {
            }
        }

        yield return null;
    }
}