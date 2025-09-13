using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NeighbourGetter
{
    public static List<EntityTransform> GetNeighboursInDistance(EntityTransform self, int distance)
    {
        // belly distance 
        if (self.IsEaten)
        {
            List<EntityTransform> inBelly = BellyManager.Instance.PlayerBelly.GetEntitiesWithin;
            var selfBellyIndex = self.BellyIndex;
            if (!selfBellyIndex.HasValue)
            {
                return new();
            }

           return inBelly.Where
            (x => x != self && x.BellyIndex != null
                            && Mathf.Abs(x.BellyIndex.Value - selfBellyIndex.Value) <= distance).ToList();
        }


        return MapManager.Instance.Entities.Where
        (x => x != self && !x.IsEaten
                        && Vector2Int.Distance(self.MapPosition, x.MapPosition) <= distance).ToList();
    }
}