using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MorselExecutor : EntityExecutor
{
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteOutsideBellyEffectCoroutine()
    {
        Debug.Log("MorselExecutor.ExecuteOutsideBellyEffectCoroutine");
        yield return Data.Damage(1, true);
        _spriteRenderer.DOKill();
        var t = 1 - ((float)Data.Health / (float)Data.MaxHealth);
        _spriteRenderer.color = Color.Lerp(Color.white, Color.red, t);
    }
}