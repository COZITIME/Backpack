using System;
using System.Collections;
using UnityEngine;

public class EntityTransform : MonoBehaviour
{
    [SerializeField]
    private float movementDuration = 0.2f;

    [SerializeField]
    private bool canBeEaten = true;

    public Vector2Int MapPosition { get; private set; }
    public int? BellyIndex { get; private set; }

    public FaceDirection Direction { get; private set; }

    private Coroutine _movementCoroutine;


    public EntityData EntityData { get; private set; }
    public EntityExecutor EntityExecutor { get; private set; }

    protected virtual void Awake()
    {
        EntityData = GetComponent<EntityData>();
        EntityExecutor = GetComponent<EntityExecutor>();
    }

    private void Start()
    {
        var vec3Pos = transform.position;
        SnapToPosition(Vector2Int.RoundToInt(vec3Pos));

        MapManager.Instance.AddEntity(this);

      
    }

    public bool IsEaten => BellyIndex.HasValue;

    public virtual bool TryMoveTo(Vector2Int position)
    {
        // check that space is free
        var mapManager = MapManager.Instance;
        if (mapManager.IsWallAtPosition(position)) return false;

        var entitiesAtTargetPosition = mapManager.GetEntitiesAtPosition(position);
        if (entitiesAtTargetPosition.Count > 0) return false;

        MapPosition = position;

        if (IsEaten)
        {
            return false;
        }

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
        }

        _movementCoroutine = StartCoroutine(MoveToPositionAnimationCoroutine());
        return true;
    }

    public bool TryTranslate(Vector2Int movement)
    {
        return TryMoveTo(movement + MapPosition);
    }

    public void FaceInDirection(FaceDirection direction)
    {
        Direction = direction;
        if (RotateSprite)
        {
            transform.rotation = Quaternion.Euler(0, 0, direction.FaceDirectionToAngle());
        }
    }


    public void SnapToPosition(Vector2Int position)
    {
        MapPosition = position;
        if (!IsEaten)
        {
            transform.position = new Vector3(MapPosition.x, MapPosition.y, 0f);
        }
    }


    public void Regurgitate(Vector2Int mouthPos, Vector2Int position, FaceDirection direction)
    {
        StopAllCoroutines();

        BellyIndex = null;
        MapPosition = position;

        FaceInDirection(direction);
        StartCoroutine(RegurgitateScaleCo(mouthPos));
    }


    protected virtual bool RotateSprite => false;


    private IEnumerator MoveToPositionAnimationCoroutine()
    {
        Vector3 start = transform.position;
        Vector3 end = (Vector3Int)MapPosition;
        yield return EntityCoroutines.MoveToPositionCoroutine(transform, movementDuration, start, end);
    }

    public void SetBellyPosition(int index, Vector2 bellyPosition)
    {
        var isAlreadyEaten = IsEaten;
        BellyIndex = index;

        if (isAlreadyEaten)
        {
            StartCoroutine(
                EntityCoroutines.MoveToPositionCoroutine(transform, .15f, transform.position, bellyPosition));
        }
        else
        {
            StartCoroutine(EatCo(bellyPosition));
        }

        FaceInDirection(FaceDirection.Down); // face down
    }

    private IEnumerator EatCo(Vector2 bellyPosition)
    {
        // scale down
        yield return EntityCoroutines.ScaleToCoroutine(transform, .2f, transform.localScale, Vector3.zero);
        transform.position = bellyPosition;
        yield return EntityCoroutines.ScaleToCoroutine(transform, .2f, transform.localScale, Vector3.one);
    }

    private IEnumerator RegurgitateScaleCo(Vector2Int mouthPos)
    {
        yield return EntityCoroutines.ScaleToCoroutine(transform, .05f, transform.localScale, Vector3.zero);

        Vector2 startPos = Vector2.Lerp(mouthPos, MapPosition, .4f);
        StartCoroutine(EntityCoroutines.MoveToPositionCoroutine(transform, .1f, startPos, (Vector2)MapPosition));

        transform.position = (Vector2)MapPosition;
        yield return EntityCoroutines.ScaleToCoroutine(transform, .1f, transform.localScale, Vector3.one);
    }

    public IEnumerator SmallPulseCoroutine()
    {
        yield return EntityCoroutines.ScaleToCoroutine(transform, .1f, transform.localScale, Vector3.one * 1.1f);
        yield return EntityCoroutines.ScaleToCoroutine(transform, .1f, transform.localScale, Vector3.one);
    }
}