using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ToolTipPanel : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private TMP_Text text;

    [SerializeField, ChildGameObjectsOnly]
    private RectTransform bg;

    private void OnValidate()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        if (bg == null)
        {
            bg = GetComponent<RectTransform>();
        }
    }

    public void SetText(string s)
    {
        text.text = s;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)text.rectTransform);
    }
}