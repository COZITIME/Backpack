using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileExecutor : EntityExecutor
{
    private EntityExecutor _shooter;

    [SerializeField]
    private int maxDistance = 30;

    private FaceDirection _shooterDirection;

    public void SetUp(EntityExecutor shooter, FaceDirection shootDirection)
    {
        EntityTransform.FaceInDirection(shootDirection);
        _shooter = shooter;
        _shooterDirection = shootDirection;

        EntityTransform.SnapToPosition(_shooter.EntityTransform.MapPosition +
                                       shootDirection.FaceDirectionToDirection());

        MapManager.Instance.Entities.Add(this.EntityTransform);
        // TurnManager.Instance.MoveToMapOrder(this);

        StartCoroutine(ExecuteMovementCoroutine());
    }

    public override IEnumerator ExecuteMovementCoroutine()
    {
        if (EntityTransform.IsEaten) yield break;

        for (var i = 0; i < maxDistance; i++)
        {
            var pos = EntityTransform.MapPosition;
            var step = pos + _shooterDirection.FaceDirectionToDirection();
            yield return null;

            if (EntityTransform.TryMoveTo(step))
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                // we are hitting something i guess
                var enemiesToHurt = MapManager.Instance.GetEntitiesAtPosition(step);
                {
                    if (EntityTransform.IsEaten) yield break;

                    foreach (var entity in enemiesToHurt)
                    {
                        if (entity == _shooter.EntityTransform) continue;

                        if (entity is PlayerTransform)
                        {
                            if (MouthHelper.IsAttackingPlayerMouth(pos))
                            {
                                if (BellyManager.Instance.PlayerBelly.TryEat(this.EntityTransform))
                                {
                                    yield break;
                                }
                            }
                        }

                        var hitData = entity.EntityData;
                        if (hitData.IsInvincibleToDamage) continue;

                        yield return hitData.Damage(damage);
                    }
                }

                yield return EntityCoroutines.MoveToPositionCoroutine(EntityTransform.transform, .1f,
                    transform.position,
                    Vector2.Lerp(transform.position, step, 0.5f));

                break;
            }
        }

        ParticleManager.Instance.PlayParticles(ParticleType.Explode, transform.position);
        SoundManager.Instance.Play(SoundType.Explode);

        yield return Data.ForceKill();
    }

    public override IEnumerator OnRegurgitatedCoroutine()
    {
        _shooter = MouthHelper.Player.EntityExecutor;
        SetUp(MouthHelper.Player.EntityExecutor, MouthHelper.Direction().ToOpposite());
        yield return null;
    }
}