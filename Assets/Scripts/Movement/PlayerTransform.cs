using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerTransform : EntityTransform
{
    public static PlayerTransform Instance { get; private set; }


    private PlayerArtHandler _playerArtHandler;


    private bool _isAwaitingInput;

    public bool DidEat { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _playerArtHandler = GetComponent<PlayerArtHandler>();
        _playerArtHandler.SetSprite(FaceDirection.Down, false);
    }

    public void DoNothingTurn()
    {
        if (!_isAwaitingInput) return;
        DidEat = false;
        _isAwaitingInput = false;
    }

    public override bool TryMoveTo(Vector2Int position)
    {
        if (!_isAwaitingInput) return false;

        // check if we are eating

        DidEat = false;
        var entitiesAtPosition = MapManager.Instance.GetEntitiesAtPosition(position);
        var count = entitiesAtPosition.Count;
        if (count > 0)
        {
            // are they in the direction? 
            if (position == MapPosition + Direction.FaceDirectionToDirection())
            {
                // can we eat them all? 
                var belly = BellyManager.Instance.PlayerBelly;
                if (belly.HasRoomForEntities(count))
                {
                    bool didEat = false;
                    for (var index = 0; index < count; index++)
                    {
                        if (belly.TryEat(entitiesAtPosition[index]))
                        {
                            didEat = true;
                        }
                    }

                    if (didEat)
                    {
                        DidEat = true;
                        SoundManager.Instance.Play(SoundType.EatEnemy);
                    }

                    return base.TryMoveTo(position); // gtfo
                }

                SoundManager.Instance.Play(SoundType.BellyFull);
            }

            _isAwaitingInput = !DidEat;
            return false;
        }

        Vector2Int oldPosition = MapPosition;
        bool didMove = base.TryMoveTo(position);
        if (!didMove) return false;

        var vomitDirection = Direction.ToOpposite();

        BellyManager.Instance.PlayerBelly.TryRegurgitate(position, oldPosition, vomitDirection, out var entityVomit);


        _isAwaitingInput = false;
        return true;
    }

    public override void FaceInDirection(FaceDirection direction)
    {
        base.FaceInDirection(direction);
        bool isEating = !BellyManager.Instance.PlayerBelly.IsFull && MapManager.Instance
            .GetEntitiesAtPosition(MapPosition + direction.FaceDirectionToDirection())
            .Any();
        _playerArtHandler.SetSprite(Direction, isEating);
    }


    public IEnumerator AwaitInputCoroutine()
    {
        _isAwaitingInput = true;
        yield return new WaitWhile(() => _isAwaitingInput);
    }
}