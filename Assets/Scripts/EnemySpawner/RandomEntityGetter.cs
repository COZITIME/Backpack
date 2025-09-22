using System.Collections.Generic;
using Cozi.Random;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Entity Getter", menuName = "Morsel Muncher/Random Entity Getter")]
public class RandomEntityGetter : ScriptableObject
{
    [SerializeField]
    private RandomWeightedCollection<EntitySpawn> weightedEntities;

    public EntityData GetEntity()
    {
        return weightedEntities.GetRandomItem(x => x.IsValid(), true).Prefab;
    }

    public EntityData GetNewEntity(List<EntityData> alreadyExistingEntities)
    {
        return weightedEntities.GetRandomItem(x => x.IsValid(alreadyExistingEntities)).Prefab;
    }
}