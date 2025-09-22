using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(DirectionalArtHandler))]
public sealed class ShooterEnemyExecutor : WalkingEnemyExecutor
{
    [BoxGroup("Move"), SerializeField]
    private float minDistance = 3f;

    [BoxGroup("Move"), SerializeField]
    private float maxDistance = 20f;

    [BoxGroup("Charge"), SerializeField, MinValue(0)]
    private int chargeDuration = 1;

    [BoxGroup("Charge"), ShowInInspector, DisplayAsString]
    private int _chargeCountDown = 1;

    [BoxGroup("Shoot"), SerializeField]
    private ProjectileExecutor projectilePrefab;


    private ShooterState _moveState;

    private DirectionalArtHandler _directionalArt;
    private FaceDirection _faceDirection;

    protected override void Awake()
    {
        base.Awake();
        _directionalArt = GetComponent<DirectionalArtHandler>();
        _moveState = ShooterState.Moving;
    }

    public override IEnumerator ExecuteMovementCoroutine()
    {
        yield return Go();
    }

    private IEnumerator Go()
    {
        if (EntityTransform.IsEaten)
        {
            _moveState = ShooterState.Moving;
            _faceDirection = FaceDirection.Down;
            _directionalArt.SetSprite(_faceDirection, false);
            yield break;
        }

        switch (_moveState)
        {
            case ShooterState.Moving:
            {
                if (IsVantagePointPredicate(EntityTransform.MapPosition)) // we are already there!
                {
                    _chargeCountDown = chargeDuration;
                    _moveState = ShooterState.Charging;
                    var playerPosition = PlayerTransform.Instance.MapPosition;
                    _faceDirection = (playerPosition - EntityTransform.MapPosition).DirectionToFaceDirection(true);
                    _directionalArt.SetSprite(_faceDirection, true);
                    yield break;
                }

                if (Pathfinder.TryGetFirstStep(EntityTransform, IsVantagePointPredicate, out var step))
                {
                    var moveDirection = step - EntityTransform.MapPosition;
                    _faceDirection = (moveDirection.DirectionToFaceDirection());
                    EntityTransform.FaceInDirection(_faceDirection);
                    yield return EntityTransform.TryMoveTo(step);
                    _directionalArt.SetSprite(_faceDirection, false);
                }

                yield break;
            }
            case ShooterState.Charging:
            {
                _directionalArt.SetSprite(_faceDirection, true);
                _chargeCountDown--;
                if (_chargeCountDown <= 0)
                {
                    _moveState = ShooterState.Shooting;
                    _chargeCountDown = chargeDuration;
                    yield return Go();
                }

                break;
            }
            case ShooterState.Shooting:
            {
                _directionalArt.SetSprite(_faceDirection, false);
                var projectile = Instantiate(projectilePrefab, (Vector2)EntityTransform.MapPosition,
                    Quaternion.identity);

                projectile.SetUp(this, _faceDirection);
                _directionalArt.SetSprite(_faceDirection, false);

                _moveState = ShooterState.Moving;
                yield break;
            }
        }
    }

    private bool IsVantagePointPredicate(Vector2Int destination)
    {
        var playerPosition = PlayerTransform.Instance.MapPosition;
        var distanceFromPlayer = Vector2Int.Distance(destination, playerPosition);

        if (distanceFromPlayer < minDistance && distanceFromPlayer > maxDistance) return false;

        var dir = (playerPosition - destination).Normalise();
        if (!MapManager.Instance.IsFree(dir + destination))
        {
            return false;
        }

        if (!IsQueenAligned(destination, playerPosition)) return false;

        if (IsObstructed(destination, playerPosition, dir)) return false;
        
        return true;
    }

    private bool IsObstructed(Vector2Int destination, Vector2Int playerPosition, Vector2Int dir)
    {
        var pos = destination;

        for (int i = 0; i < 50; i++)
        {
            pos += dir;
            if (pos == playerPosition) return false;

            if (!MapManager.Instance.IsFree(pos)) // if we hit something
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsQueenAligned(Vector2Int a, Vector2Int b)
    {
        return a.x == b.x // same column
               || a.y == b.y // same row
               || Mathf.Abs(a.x - b.x) == Mathf.Abs(a.y - b.y); // same diagonal
    }

    private enum ShooterState
    {
        Moving,
        Charging,
        Shooting,
    }
}