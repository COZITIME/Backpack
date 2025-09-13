using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField]
    private float bellyDelay = .1f;

    [SerializeField]
    private float onMapDelay = .1f;

    [SerializeField]
    private PlayerTransform player;

    private bool isGameOver = false;

    private readonly List<EntityExecutor> _entitiesOnMap = new List<EntityExecutor>();
    private readonly List<EntityExecutor> _entitiesInBelly = new List<EntityExecutor>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(RunCoroutine());

        PlayerTransform.Instance.EntityData.OnHealthChange += OnHealthChange;
    }

    private void OnHealthChange(int health)
    {
        if (health > 0) return;
        isGameOver = true;
        this.ExecuteDelayedRealtime(2f, () => { GameOverManager.Instance.GameOver(); });
    }

    public void MoveToBellyOrder(EntityExecutor entityExecutor, bool moveToTop = true)
        => MoveEntity(_entitiesOnMap, _entitiesInBelly, entityExecutor, moveToTop);

    public void MoveToMapOrder(EntityExecutor entityExecutor, bool moveToTop = false)
        => MoveEntity(_entitiesInBelly, _entitiesOnMap, entityExecutor, moveToTop);

    /// <summary>
    /// Moves an entity between two lists, optionally placing it at the top.
    /// </summary>
    private void MoveEntity(List<EntityExecutor> fromList, List<EntityExecutor> toList, EntityExecutor entity,
        bool moveToTop)
    {
        if (fromList.Contains(entity)) fromList.Remove(entity);
        if (toList.Contains(entity)) toList.Remove(entity);

        if (moveToTop)
            toList.Insert(0, entity);
        else
            toList.Add(entity);
    }

    private IEnumerator RunCoroutine()
    {
        while (true)
        {
            if (isGameOver) break;

            // in belly first
            var waitForBellyDelay = new WaitForSeconds(bellyDelay);
            for (var index = 0; index < _entitiesInBelly.Count; index++)
            {
                var entity = _entitiesInBelly[index];
                yield return entity.ExecuteInsideBellyEffectCoroutine();
                yield return waitForBellyDelay;
            }

            yield return null;

            // enemies on map

            var waitForMapDelay = new WaitForSeconds(onMapDelay);
            for (var index = 0; index < _entitiesOnMap.Count; index++)
            {
                EntityExecutor entity = _entitiesOnMap[index];
                if (!entity) continue;
                yield return entity.ExecuteOutsideBellyEffectCoroutine();
                if (!entity.Data.IsDead)
                {
                    yield return entity.ExecuteMovementCoroutine();
                }

                yield return waitForMapDelay;

                // if we are a player and ate skip turns
                if (entity.EntityTransform is PlayerTransform playerTransform)
                {
                    if (playerTransform.DidEat)
                    {
                        break;
                    }
                }
            }

            ProgressionManager.Instance.OnTurnFinished();

            yield return null;
        }
    }

    public void RemoveEntity(EntityExecutor entity)
    {
        if (_entitiesOnMap.Contains(entity)) _entitiesOnMap.Remove(entity);
        if (_entitiesInBelly.Contains(entity)) _entitiesInBelly.Remove(entity);
    }


    public List<EntityExecutor> GetEntities(bool inMap = true, bool inBelly = true)
    {
        var result = new List<EntityExecutor>();
        if (inMap) result.AddRange(_entitiesOnMap);
        if (inBelly) result.AddRange(_entitiesInBelly);
        return result;
    }
}