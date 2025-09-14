using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class WalkingEnemyExecutor : EntityExecutor
{
    [SerializeField, ChildGameObjectsOnly]
    protected EnemyMove mover;

    protected override void Awake()
    {
        base.Awake();
        mover = GetComponent<EnemyMove>();
    }

    public override IEnumerator ExecuteMovementCoroutine()
    {
        if (IsMovementStunned)
        {
            yield return StartMovementStunCoroutine(-1, true);
            yield break;
        }

        if (Data.Traits.HasFlag(Trait.Melee) && !PlayerTransform.Instance.EntityData.IsDead)
        {
            bool isBesidePlayer =
                Pathfinder.IsNeighbour(EntityTransform.MapPosition, PlayerTransform.Instance.MapPosition);

            if (isBesidePlayer)
            {
                yield return MeleePlayerCoroutine();
                yield break;
            }
        }

        if (mover)
        {
            yield return mover.TryMoveToPlayer();
        }
    }

    private IEnumerator MeleePlayerCoroutine()
    {
        var startPosition = (Vector2)EntityTransform.MapPosition;
        var playerPosition = PlayerTransform.Instance.MapPosition;
        var damagePosition = Vector2.Lerp(startPosition, playerPosition, 0.4f);

        yield return EntityCoroutines.MoveToPositionCoroutine(EntityTransform.transform, 0.1f, startPosition,
            damagePosition);
        yield return PlayerTransform.Instance.EntityData.Damage(damage);
        yield return EntityCoroutines.MoveToPositionCoroutine(EntityTransform.transform, 0.1f, transform.position,
            startPosition);
    }
}