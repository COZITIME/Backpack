using System;
using System.Collections;
using UnityEngine;

public class PlayerTransform : EntityTransform
{
    public static PlayerTransform Instance { get; private set; }

    private bool _isAwaitingInput;

    public bool DidEat { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override bool TryMoveTo(Vector2Int position)
    {
        if (!_isAwaitingInput) return false;

        DidEat = false;
        // check if we are eating
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
                        SoundManager.Instance.Play(SoundType.PlayerEat);
                    }

                    return base.TryMoveTo(position); // gtfo
                }
            }

            _isAwaitingInput = false;
            return false;
        }

        Vector2Int oldPosition = MapPosition;
        bool didMove = base.TryMoveTo(position);
        if (!didMove) return false;

        var vomitDirection = Direction.ToOpposite();
        bool isRegurgitating = position == oldPosition + vomitDirection.FaceDirectionToDirection();
        if (isRegurgitating)
        {
            if (BellyManager.Instance.PlayerBelly.TryRegurgitate(position, oldPosition, vomitDirection,
                    out var entityVomit))
            {
                SoundManager.Instance.Play(SoundType.PlayerRegurgitate);
            }
        }

        _isAwaitingInput = false;
        return true;
    }

    protected override bool RotateSprite => true;


    public IEnumerator AwaitInputCoroutine()
    {
        _isAwaitingInput = true;
        yield return new WaitWhile(() => _isAwaitingInput);
    }
}