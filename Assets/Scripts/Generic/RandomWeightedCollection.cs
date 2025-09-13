using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Cozi.Random.SingleItem;

namespace Cozi.Random
{
    /// <summary>
    /// For a collection of items with different weights, this class will return a random item based on the weights.
    /// </summary>
    [System.Serializable]
    public class RandomWeightedCollection<T>
    {
        
        public RandomWeightedCollection(List<T> list)
        {
            items = new List<RandomWeightedItem<T>>();
            foreach (var item in list)
            {
                items.Add(new RandomWeightedItem<T>(item, 1f));
            }
        }
        
        public delegate bool ItemPredicate(T item);

        [SerializeField]
        private List<RandomWeightedItem<T>> items = new();

        [Button("Order By Weight")]
        private void OrderByWeight()
        {
            items.Sort((a, b) => b.Weight.CompareTo(a.Weight));
        }

        /// <summary>
        /// Gets a random item from the collection based on the weights of the items.
        /// </summary>
        /// <returns>Random T item in collection.</returns>
        public T GetRandomItem()
        {
            float totalWeight = GetTotalWeight();
   
            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float weightSum = 0;

            int length = items.Count;
            for (int i = 0; i < length; i++)
            {
                weightSum += items[i].Weight;
                if (randomValue <= weightSum)
                {
                    return items[i].Item;
                }
            }

            Debug.LogError("RandomWeightedCollection: GetRandomItem() failed to return an item. :(");
            return default;
        }


        /// <summary>
        /// Gets a random item from the collection filtered down based on the predicate and the weights of the items.
        /// </summary>
        /// <param name="predicate">The predicate that the selected item must match</param>
        /// <param name="tryWithoutPredicateOnFail">if true, if no match is found with predicate it will run check again without the predicate.</param>
        /// <returns>Random T item in collection.</returns>
        public T GetRandomItem(ItemPredicate predicate, bool tryWithoutPredicateOnFail = false)
        {
            float totalWeight = GetTotalWeight(predicate);

            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float weightSum = 0;

            List<T> checkedItems = new();

            int length = items.Count;
            for (int i = 0; i < length; i++)
            {
                var randomWeightedItem = items[i];
                if (!predicate(randomWeightedItem.Item))
                {
                    continue;
                }

                weightSum += randomWeightedItem.Weight;
                if (randomValue <= weightSum)
                {
                    return randomWeightedItem.Item;
                }
            }

            if (tryWithoutPredicateOnFail)
            {
                return GetRandomItem();
            }

            Debug.LogError("RandomWeightedCollection: GetRandomItemWithPredicate() failed to return an item. " +
                           "Perhaps no item matches the predicate");
            return default;
        }

        /// <summary>
        /// Returns the total weight of all items in the collection with a predicate.
        /// </summary>
        private float GetTotalWeight(ItemPredicate predicate)
        {
            float totalWeight = 0;
            int length = items.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(items[i].Item))
                {
                    totalWeight += items[i].Weight;
                }
            }

            return totalWeight;
        }

        /// <summary>
        /// Returns the total weight of all items in the collection.
        /// </summary>
        private float GetTotalWeight()
        {
            float totalWeight = 0;
            int length = items.Count;
            for (int i = 0; i < length; i++)
            {
                totalWeight += items[i].Weight;
            }

            return totalWeight;
        }


        [OnInspectorGUI]
        private void OnItemsChanged()
        {
            float totalWeight = GetTotalWeight();
            int length = items.Count;
            for (int i = 0; i < length; i++)
            {
                float fraction = items[i].Weight / totalWeight;
                items[i].SetFractionBar(fraction);
            }
        }


        public void AddItem(T item, float weight)
        {
            items.Add(new RandomWeightedItem<T>(item, weight));
        }
    }
}