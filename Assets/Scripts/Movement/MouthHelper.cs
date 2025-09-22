using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MouthHelper
{
    public static PlayerTransform Player => PlayerTransform.Instance;

    public static bool IsAttackingPlayerMouth(Vector2Int attackerPosition)
    {
        var playerPosition = Player.MapPosition;
        FaceDirection directionFromPlayerToAttack = (attackerPosition - playerPosition).DirectionToFaceDirection(true);
        if (IsMouthDirection(directionFromPlayerToAttack)) return true;
        return false;
    }

    public static FaceDirection MouthDirection()
    {
        return Player.Direction;
    }

    public static Vector2Int MouthPosition()
    {
        return Player.MapPosition + MouthDirection().FaceDirectionToDirection();
    }

    public static bool IsMouthDirection(FaceDirection direction)
    {
        return direction == MouthDirection();
    }

    public static int GetMouthBlockedDamage(Vector2Int attackerPos, int damage)
    {
        if (IsAttackingPlayerMouth(attackerPos))
        {
            var resistance = RelicManager.Instance.GetRelicCount(RelicType.MouthShield);
            damage -= resistance;
            damage = Math.Max(0, damage);
        }

        return damage;
    }

    public static bool IsBellyFull => BellyManager.Instance.PlayerBelly.IsFull;


    public static List<FaceDirection> GetWalkDirections()
    {
        var list = new List<FaceDirection>();
        foreach (var dir in FaceDirectionUtils.GetAllDirections())
        {
            var spot = Player.MapPosition + dir.FaceDirectionToDirection();
            if (MapManager.Instance.IsFree(spot, true, true))
            {
                list.Add(dir);
            }
        }

        return list;
    }

    public static List<FaceDirection> GetEatDirections()
    {
        if (BellyManager.Instance.PlayerBelly.IsFull) return new List<FaceDirection>();
        var list = new List<FaceDirection>();
        foreach (var dir in FaceDirectionUtils.GetAllDirections())
        {
            var spot = Player.MapPosition + dir.FaceDirectionToDirection();
            var entitiesAtPosition = MapManager.Instance.GetEntitiesAtPosition(spot);
            var amount = entitiesAtPosition.Count;
            if (amount != 0 && BellyManager.Instance.PlayerBelly.HasRoomForEntities(amount))
            {
                list.Add(dir);
            }
        }

        return list;
    }
}

public static class FaceDirectionUtils
{
    public static List<FaceDirection> GetAllDirections()
    {
        return Enum.GetValues(typeof(FaceDirection))
            .Cast<FaceDirection>()
            .ToList();
    }
}