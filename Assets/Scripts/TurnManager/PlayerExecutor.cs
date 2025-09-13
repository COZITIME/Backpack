using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerTransform))]
public class PlayerExecutor : EntityExecutor
{
    private PlayerTransform _playerTransform;

    protected override void Awake()
    {
        base.Awake();
        _playerTransform = EntityTransform as PlayerTransform;
    }

    public override IEnumerator ExecuteMovementCoroutine()
    {
        yield return _playerTransform.AwaitInputCoroutine();
    }

    protected override bool AddToTop => true;
}