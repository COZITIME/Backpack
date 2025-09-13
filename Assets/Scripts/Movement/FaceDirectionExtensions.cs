using System;
using UnityEngine;

public static class FaceDirectionExtensions
{
    public static Vector2Int FaceDirectionToDirection(this FaceDirection faceDirection)
    {
        return faceDirection switch
        {
            FaceDirection.Up => Vector2Int.up,
            FaceDirection.Down => Vector2Int.down,
            FaceDirection.Left => Vector2Int.left,
            FaceDirection.Right => Vector2Int.right,
            FaceDirection.LeftUp => new Vector2Int(-1, 1),
            FaceDirection.LeftDown => new Vector2Int(-1, -1),
            FaceDirection.RightUp => new Vector2Int(1, 1),
            FaceDirection.RightDown => new Vector2Int(1, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
        };
    }

    public static FaceDirection DirectionToFaceDirection(this Vector2Int direction)
    {
        return direction switch
        {
            { x: 0, y: 1 } => FaceDirection.Up,
            { x: 0, y: -1 } => FaceDirection.Down,
            { x: -1, y: 0 } => FaceDirection.Left,
            { x: 1, y: 0 } => FaceDirection.Right,

            { x: -1, y: 1 } => FaceDirection.LeftUp,
            { x: -1, y: -1 } => FaceDirection.LeftDown,
            { x: 1, y: 1 } => FaceDirection.RightUp,
            { x: 1, y: -1 } => FaceDirection.RightDown,

            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction,
                "Direction must be one of the 8 cardinal/diagonal unit vectors.")
        };
    }


    public static float FaceDirectionToAngle(this FaceDirection faceDirection)
    {
        return faceDirection switch
        {
            FaceDirection.Up => 0f,
            FaceDirection.RightUp => -45f,
            FaceDirection.Right => -90f,
            FaceDirection.RightDown => -135f,
            FaceDirection.Down => 180f,
            FaceDirection.LeftDown => 135f,
            FaceDirection.Left => 90f,
            FaceDirection.LeftUp => 45f,
            _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
        };
    }

    public static FaceDirection ToOpposite(this FaceDirection faceDirection)
    {
        return faceDirection switch
        {
            FaceDirection.Up        => FaceDirection.Down,
            FaceDirection.Down      => FaceDirection.Up,
            FaceDirection.Left      => FaceDirection.Right,
            FaceDirection.Right     => FaceDirection.Left,

            FaceDirection.LeftUp    => FaceDirection.RightDown,
            FaceDirection.LeftDown  => FaceDirection.RightUp,
            FaceDirection.RightUp   => FaceDirection.LeftDown,
            FaceDirection.RightDown => FaceDirection.LeftUp,

            _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
        };
    }

}