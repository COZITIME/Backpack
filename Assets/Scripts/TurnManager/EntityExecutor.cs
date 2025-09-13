using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using Sirenix.OdinInspector;
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

    public int MovementStun = 0;

    protected virtual void Awake()
    {
        Data = GetComponent<EntityData>();
        EntityTransform = GetComponent<EntityTransform>();
        Sprite = GetComponent<SpriteRenderer>();
    }


    public void Start()
    {
        TurnManager.Instance.MoveToMapOrder(this, AddToTop);
    }

    protected virtual bool AddToTop => false;


    public bool IsMovementStunned => MovementStun > 0;

    public virtual IEnumerator TickDownMovementStunCoroutine()
    {
        MovementStun--;
        yield return Sprite.DOColor(IsMovementStunned ? Color.grey : Color.white, .5f);
    }

    public virtual IEnumerator StartMovementStunCoroutine(int stunAmount, bool isAdditive = false)
    {
        if (isAdditive)
        {
            MovementStun += stunAmount;
        }
        else
        {
            MovementStun = stunAmount;
        }

        yield return Sprite.DOColor(IsMovementStunned ? Color.grey : Color.white, .5f);
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
        yield return null;
    }

    public virtual IEnumerator OnEatCoroutine()
    {
        yield return null;
    }

    public virtual IEnumerator OnRegurgitateCoroutine()
    {
        yield return null;
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

        if (traitFlags.HasFlag(Trait.Burner))
        {
            var neighbours = NeighbourGetter.GetNeighboursInDistance(EntityTransform, 1);
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
    }

    public virtual IEnumerator BurnEntity(EntityData enemy)
    {
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
}