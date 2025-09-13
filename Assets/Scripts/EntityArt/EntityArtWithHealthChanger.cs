using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


public class EntityArtWithHealthChanger : MonoBehaviour
{
    [SerializeField]
    private Sprite aboveHealthSprite;

    [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowIndexLabels = true)]
    private Sprite[] spritesAtHealth;

    private EntityData _entityData;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _entityData = GetComponent<EntityData>();
        _entityData.OnHealthChange += OnHealthChange;
    }

    private void Start()
    {
        OnHealthChange(_entityData.Health);
    }

    private void OnHealthChange(int health)
    {
        _spriteRenderer.sprite = GetSprite(health);
    }

    private Sprite GetSprite(int health)
    {
        var sprite = aboveHealthSprite;
        if (health + 1 > spritesAtHealth.Length)
        {
            return aboveHealthSprite;
        }

        for (var i = 0; i < spritesAtHealth.Length; i++)
        {
            if (!spritesAtHealth[i]) continue;
            if (i > health) break;
            sprite = spritesAtHealth[i];
        }

        return sprite;
    }
}