using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthUiHandler : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image healthPrefab;

    [SerializeField]
    private RectTransform healthRect;

    [SerializeField]
    private EntityData entityData;

    [SerializeField]
    private Color aliveColor = Color.white;

    [SerializeField]
    private Color deadColor = Color.red;

    private Image[] _hearts = Array.Empty<Image>();

    private void Start()
    {
        entityData.OnHealthChange += UpdateHealth;
        UpdateHealth(entityData.Health);
    }

    private void UpdateHealth(int hp)
    {
        int heartsRequired = Mathf.Max(hp, entityData.MaxHealth);
        if ((_hearts.Length != heartsRequired))
        {
            SpawnNewHearts(heartsRequired);
        }

        for (int i = 0; i < _hearts.Length; i++)
        {
            var newColour = i < hp ? aliveColor : deadColor;
            if (_hearts[i].color != newColour)
            {
                _hearts[i].DOColor(newColour, .2f);
                _hearts[i].transform.DOPunchScale(Vector3.one * 1.2f, .2f);
            }
        }
    }

    private void SpawnNewHearts(int amount)
    {
        DestroyOld();
        _hearts = new UnityEngine.UI.Image[amount];
        for (int i = 0; i < amount; i++)
        {
            _hearts[i] = Instantiate(healthPrefab, healthRect);
        }
    }

    private void DestroyOld()
    {
        for (int i = healthRect.childCount - 1; i >= 0; i--)
        {
            Destroy(healthRect.GetChild(i).gameObject);
        }
    }
}