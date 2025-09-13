using Sirenix.OdinInspector;
using UnityEngine;
namespace Cozi.Random
{
    /// <summary>
    /// Random chance struct. Used to determine if a random event should happen. 1 = always, 0 = never.
    /// </summary>
    [InlineProperty, System.Serializable]
    public struct RandomChance
    {
        [Range(0, 1), Tooltip("Odds of event happening, 1 = always, 0 = never"), HideLabel]
        public float Chance;

        public RandomChance(float chance)
        {
            Chance = chance;
        }

        public readonly bool IsSuccess() => UnityEngine.Random.value <= Chance;
    }
}