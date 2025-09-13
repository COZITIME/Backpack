using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Cozi.Random.SingleItem
{


    /// <summary>
    /// An item within a <see cref="RandomWeightedCollection{T}"/>
    /// <typeparam name="T"></typeparam>
    [System.Serializable, InlineProperty, HideLabel]
    public class RandomWeightedItem<T>
    {
        #region Odin

        private bool showFractionBar = false;

        [SerializeField, ReadOnly, HideLabel, ProgressBar(0, 1f, r: 1, g: 1, b: 1, DrawValueLabel = false, Height = 6),
         ShowIf(nameof(showFractionBar))]
        private float fraction;

        #endregion

        [HorizontalGroup("RandomWeightedItem", Width = 100, LabelWidth = 20),
         LabelText("W:"), MinValue(0), Tooltip("The higher the value the more likely this item is to be picked.")]
        public float Weight = 1;

        [HorizontalGroup("RandomWeightedItem", LabelWidth = 35), LabelText("Item:"),
         Tooltip("The item itself."), InlineProperty]
        public T Item;

        public RandomWeightedItem(T item, float weight)
        {
            this.Item = item;
            this.Weight = weight;
        }

        internal void SetFractionBar(float fraction)
        {
            showFractionBar = true;
            this.fraction = fraction;
        }
    }
}