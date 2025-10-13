using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerTransform : EntityTransform
{
    public static PlayerTransform Instance { get; private set; }


    private DirectionalArtHandler _directionalArtHandler;

    [SerializeField]
    private bool freeVomitMove = true;

    [SerializeField]
    private bool freeEatMove;


    private bool _isAwaitingInput;

    public bool GetExtraTurn { get; set; }
    public bool AteLastTurn { get; set; }


    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _directionalArtHandler = GetComponent<DirectionalArtHandler>();
        _directionalArtHandler.SetSprite(FaceDirection.Down, true);
    }

    public void DoNothingTurn()
    {
        if (!_isAwaitingInput) return;

        GetExtraTurn = false;
        _isAwaitingInput = false;

        HandleChew();

        _directionalArtHandler.SetSprite(Direction, !MouthHelper.IsBellyFull);

        ArrowButtonManager.Instance.UpdateButtons();
    }

    private void HandleChew()
    {
        var inBelly = BellyManager.Instance.PlayerBelly.GetEntitiesWithin;
        var within = inBelly.Where(entity => entity.EntityData.IsMorsel).ToArray();

        var chewDamage = RelicManager.Instance.GetRelicCount(RelicType.ChewDamage);
        bool dealDamage = chewDamage > 0;

        foreach (var entity in within)
        {
            var insideData = entity.EntityData;
            if (insideData.IsMorsel)
            {
                insideData.TryEatMorsel();
            }
            else if (!insideData.IsInvincibleToDamage)
            {
                if (dealDamage)
                {
                    StartCoroutine(insideData.Damage(chewDamage));
                }
            }
        }
    }

    public override bool TryMoveTo(Vector2Int position)
    {
        ArrowButtonManager.Instance.UpdateButtons();
        if (!_isAwaitingInput) return false;

        _isAwaitingInput = false;
        AteLastTurn = false;
        GetExtraTurn = false;

        // check if we are eating
        if (!MouthHelper.IsBellyFull)
        {
            if (HandleEatingMovement(position))
            {
                return true;
            }
        }

        // if moving into object that we cant eat 
        if (!MapManager.Instance.IsFree(position))
        {
            _isAwaitingInput = true;
            GetExtraTurn = true;
            return false;
        }

        // try move and to vomit
        Vector2Int oldPosition = MapPosition;
        bool didMove = base.TryMoveTo(position);
        if (!didMove)
        {
            _isAwaitingInput = true;
            return false;
        }

        var vomitDirection = Direction.ToOpposite();
        bool didVomit = BellyManager.Instance.PlayerBelly.TryRegurgitate(position, oldPosition, vomitDirection, out _);
        if (didVomit)
        {
            GetExtraTurn = freeVomitMove;
        }

        var dir = didVomit ? Direction.ToOpposite() : Direction;
        FaceInDirection(dir);
        
        ArrowButtonManager.Instance.UpdateButtons();
        _isAwaitingInput = false;
        return true;
    }

    private bool HandleEatingMovement(Vector2Int position)
    {
        var didMove = false;
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
                        AteLastTurn = true;
                        if (freeEatMove) GetExtraTurn = true;
                        SoundManager.Instance.Play(SoundType.EatEnemy);
                    }

                    ArrowButtonManager.Instance.UpdateButtons();

                    if (MouthHelper.IsBellyFull)
                    {
                        _directionalArtHandler.SetSprite(Direction, false);
                    }

                    var wasAbleToMove = base.TryMoveTo(position);
                    if (!wasAbleToMove)
                    {
                        _isAwaitingInput = true;
                    }

                    didMove = wasAbleToMove;
                    return true;
                }

                SoundManager.Instance.Play(SoundType.BellyFull);
            }

            if (freeEatMove)
            {
                _isAwaitingInput = !GetExtraTurn;
            }
        }

        return didMove;
    }

    public override void FaceInDirection(FaceDirection direction)
    {
        // bool goBackwards =
        //     !BellyManager.Instance.PlayerBelly.IsEmpty
        //     && (BellyManager.Instance.PlayerBelly.IsFull
        //         || !MapManager.Instance
        //             .GetEntitiesAtPosition(MapPosition + direction.FaceDirectionToDirection()).Any());

        
        base.FaceInDirection(direction);
       
        _directionalArtHandler.SetSprite(Direction, !BellyManager.Instance.PlayerBelly.IsFull);
    }


    public IEnumerator AwaitInputCoroutine()
    {
        _isAwaitingInput = true;
        yield return new WaitWhile(() => _isAwaitingInput);
    }
}