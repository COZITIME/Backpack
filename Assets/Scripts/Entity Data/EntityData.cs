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
    public EntityTransform EntityTransform { get; private set; }
    public EntityExecutor Executor { get; private set; }
    
    private bool _isKilled = false;

    public int MaxHealth => maxHealth;
    public int Health => _health;
    public Trait Traits => traits;
    public string ColouredName => entityName.ToColouredString(nameColour);
    public bool IsDead => _health <= 0;
    public bool IsInvincible => traits.HasFlag(Trait.Relic);
    public string Description => description;
    
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
        TurnManager.Instance.RemoveEntity(Executor);
        BellyManager.Instance.PlayerBelly.Remove(EntityTransform);
        MapManager.Instance.RemoveEntity(EntityTransform);
    }
}