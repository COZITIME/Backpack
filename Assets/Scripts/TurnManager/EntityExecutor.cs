using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


public class EntityExecutor : MonoBehaviour
{
    public EntityData Data { get; private set; }
    public EntityTransform EntityTransform { get; private set; }

    public SpriteRenderer Sprite { get; private set; }

    [SerializeField]
    public int damage = 1;

    [SerializeField]
    private SoundType hurtSound = SoundType.EnemyHurt;

    [SerializeField]
    private SoundType deathSound = SoundType.EnemyDied;

    [SerializeField]
    private SoundType eatSound = SoundType.EatEnemy;

    [SerializeField]
    private SoundType regurgitateSound = SoundType.EnemyDied;

    public int MovementStun = 0;

    protected virtual void Awake()
    {
        Data = GetComponent<EntityData>();
        EntityTransform = GetComponent<EntityTransform>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual bool AddToTop => false;

    public bool IsMovementStunned => MovementStun > 0;

    public virtual IEnumerator TickDownMovementStunCoroutine()
    {
        MovementStun--;
        yield return null;
        //  yield return Sprite.DOColor(IsMovementStunned ? Color.black : Color.white, .5f);
    }

    public virtual IEnumerator StartMovementStunCoroutine(int stunAmount, bool isAdditive = false)
    {
        if (isAdditive)
        {
            MovementStun += stunAmount;
        }
        else
        {
            MovementStun = math.max(MovementStun, stunAmount);
        }

        yield return null;
        //yield return Sprite.DOColor(IsMovementStunned ? Color.black : Color.white, .5f);
    }

    public virtual IEnumerator ExecuteInsideBellyEffectCoroutine()
    {
        StartCoroutine(EntityTransform.SmallPulseCoroutine());
        yield return HandleTraitsCoroutine(true);
        yield return null;
    }

    public virtual IEnumerator ExecuteOutsideBellyEffectCoroutine()
    {
        StartCoroutine(EntityTransform.SmallPulseCoroutine());
        yield return HandleTraitsCoroutine(false);
        yield return null;
    }

    public virtual IEnumerator ExecuteMovementCoroutine()
    {
        if (IsMovementStunned)
        {
            yield return StartMovementStunCoroutine(-1);
            yield break;
        }

        yield return null;
    }

    public virtual IEnumerator OnEatenCoroutine()
    {
        int d = RelicManager.Instance.GetRelicCount(RelicType.BiteDamage);
        if (d > 0)
        {
            yield return Data.Damage(d);
        }

        SoundManager.Instance.Play(eatSound);
    }

    public virtual IEnumerator OnRegurgitatedCoroutine()
    {
        int d = RelicManager.Instance.GetRelicCount(RelicType.RegurgeDamage);
        if (d > 0)
        {
            yield return Data.Damage(d);
        }

        SoundManager.Instance.Play(regurgitateSound);
    }

    public virtual IEnumerator OnKilledCoroutine()
    {
        this.DOKill(false);
        Sprite.DOColor(Color.red, 0.2f);

        StartCoroutine(EntityCoroutines.ScaleToCoroutine(transform, .5f, transform.localScale, Vector3.zero));

        if (Data.Traits.HasFlag(Trait.Bomb))
        {
            yield return ExplodeCoroutine();
        }

        yield return null;
    }

    public virtual IEnumerator OnDamagedCoroutine()
    {
        this.DOKill(false);
        var colourTween = Sprite.DOColor(Color.red, 0.2f);
        colourTween.onComplete += () => colourTween.Rewind();

        SoundManager.Instance.Play(hurtSound);

        yield return EntityCoroutines.ScaleToCoroutine(transform, .1f, transform.localScale, Vector3.one * .5f);
        yield return EntityCoroutines.ScaleToCoroutine(transform, .2f, transform.localScale, Vector3.one);
        yield return null;
    }

    public virtual IEnumerator HandleTraitsCoroutine(bool isInBelly)
    {
        var traitFlags = Data.Traits;

        if (!IsMovementStunned && traitFlags.HasFlag(Trait.Burner))
        {
            ParticleManager.Instance.PlayParticles(ParticleType.BurnSmall, transform.position);
            var neighbours = NeighbourGetter.GetNeighboursInDistance(EntityTransform, 1.5f);
            foreach (var neighbour in neighbours)
            {
                EntityData neighbourEntityData = neighbour.EntityData;
                if (neighbourEntityData.Traits.HasFlag(Trait.Fireproof) ||
                    neighbourEntityData.Traits.HasFlag(Trait.Burner))
                {
                    continue;
                }

                yield return BurnEntity(neighbourEntityData);
            }
        }

        if (traitFlags.HasFlag(Trait.Bomb))
        {
            yield return Data.Damage(1);
        }

        if (traitFlags.HasFlag(Trait.Morsel) && isInBelly)
        {
            this.Data.TryEatMorsel();
        }
    }

    public virtual IEnumerator BurnEntity(EntityData enemy)
    {
        if (enemy.IsInvincible) yield break;
        var part = ParticleManager.Instance.PlayParticles(ParticleType.Burn, transform.position);
        yield return part.transform.DOMove(enemy.transform.position, .2f);
        SoundManager.Instance.Play(hurtSound);
        yield return enemy.Damage(damage);
    }

    private IEnumerator ExplodeCoroutine()
    {
        ParticleManager.Instance.PlayParticles(ParticleType.Explode, EntityTransform.transform.position);
        SoundManager.Instance.Play(deathSound);
        var neighbours = NeighbourGetter.GetNeighboursInDistance(EntityTransform, 2);
        foreach (var neighbour in neighbours)
        {
            yield return new WaitForSeconds(.2f);
            yield return (neighbour.EntityData.Damage(damage));
        }

        yield return new WaitForSeconds(.3f);
    }

    public void SetStun(int i)
    {
        StartCoroutine(StartMovementStunCoroutine(i));
    }

    public IEnumerator GainHealthCoroutine(int health)
    {
        yield return Sprite.DOColor(Color.green, 0.2f);
        yield return Sprite.DOColor(Color.white, 0.2f);
    }
}