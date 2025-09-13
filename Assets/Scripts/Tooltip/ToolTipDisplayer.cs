using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipDisplayer : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private EntityData entity;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var lines = new List<string>();

        if (entity != null)
        {
            string headerMessage = entity.ColouredName;
            if (!entity.IsInvincible) headerMessage += $" ({entity.Health}/{entity.MaxHealth})";
            if (!string.IsNullOrWhiteSpace(entity.Description))
                headerMessage += $"\n{entity.Description}";
            lines.Add(headerMessage);

            foreach (Trait t in System.Enum.GetValues(typeof(Trait)))
            {
                if (t != Trait.None && entity.Traits.HasFlag(t))
                {
                    string message = $"{t.ToColouredString(t.GetTraitColor()).ToBoldString()}" +
                                     $"\n{t.GetTraitDescription()}";
                    lines.Add(message);
                }
            }
        }


        ToolTipManager.I.Show(lines);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.I.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // Manager handles positioning each frame, so nothing needed here
    }
}