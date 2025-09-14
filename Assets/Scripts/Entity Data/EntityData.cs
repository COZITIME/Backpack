using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityData : MonoBehaviour
{
    public event Action<int> OnHealthChange;

    [BoxGroup("Description"), SerializeField]
    private string entityName;

    [BoxGroup("Description"), SerializeField]
    private Color nameColour = Color.darkGreen;

    [BoxGroup("Description"), SerializeField, MultiLineProperty(3)]
    private string description;

    [BoxGroup("Traits"), SerializeField]
    private Trait traits;

    [BoxGroup("Health"), SerializeField, HideIf(nameof(IsInvincible))]
    private int maxHealth;

    [BoxGroup("Health"), ShowInInspector, DisplayAsString]
    private int _health;

    [BoxGroup("Morsel"), SerializeField, ShowIf(nameof(IsMorsel))]
    private int morselSustenance = 1;

    [FormerlySerializedAs("health")] [BoxGroup("Morsel"), SerializeField, ShowIf(nameof(IsMorsel))]
    private int morselHealth = 0;

    [BoxGroup("Relic"), SerializeField, ShowIf(nameof(IsRelic))]
    public RelicType RelicType = RelicType.BellyUp;

    public EntityTransform EntityTransform { get; private set; }
    public EntityExecutor Executor { get; private set; }

    private bool _isKilled = false;

    public int MaxHealth => maxHealth;
    public int Health => _health;
    public Trait Traits => traits;
    public string ColouredName => entityName.ToColouredString(nameColour);
    public bool IsDead => _health <= 0;
    public bool IsInvincible => IsRelic || IsMorsel;
    public string Description => description;
    public bool IsMorsel => traits.HasFlag(Trait.Morsel);
    public bool IsRelic => traits.HasFlag(Trait.Relic);

    public bool IsMaxHealth => _health >= maxHealth;


    private void Awake()
    {
        EntityTransform = GetComponent<EntityTransform>();
        Executor = GetComponent<EntityExecutor>();
        _health = maxHealth;
    }

    public IEnumerator Damage(int amount)
    {
        if (IsInvincible) yield break;
        if (_isKilled) yield break;

        _health -= amount;
        if (_health <= 0)
        {
            Kill();
            _isKilled = true;
        }

        OnHealthChange?.Invoke(_health);

        yield return IsDead ? Executor.OnKilledCoroutine() : Executor.OnDamagedCoroutine();
    }

    private void Kill()
    {
        if (!traits.HasFlag(Trait.Morsel) && !traits.HasFlag(Trait.Relic))
        {
            MorselSpawningManager.Instance.OnSpawnFromEntity(EntityTransform);
        }

        TurnManager.Instance.RemoveEntity(Executor);
        BellyManager.Instance.PlayerBelly.Remove(EntityTransform);
        MapManager.Instance.RemoveEntity(EntityTransform);
    }

    public void TryEatMorsel()
    {
        if (morselHealth > 0 && PlayerTransform.Instance.EntityData.IsMaxHealth)
        {
            return; // only eat health ones if not at max health
        }

        SoundManager.Instance.Play(SoundType.PlayerDigest);

        PlayerTransform.Instance.EntityData.AddHealth(morselHealth);
        XpManager.Instance.GainXp(morselSustenance);

        _health = 0;
        Kill();
        OnHealthChange?.Invoke(_health);
        StartCoroutine(Executor.OnKilledCoroutine());
    }


    private void AddHealth(int amount)
    {
        if (amount <= 0) return;
        _health += amount;
        _health = Mathf.Clamp(_health, 0, maxHealth);
        OnHealthChange?.Invoke(_health);
        StartCoroutine(Executor.GainHealthCoroutine(_health));
    }
}