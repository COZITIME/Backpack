using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager I { get; private set; }


    [SerializeField]
    private ToolTipPanel tooltipPrefab;

    [SerializeField]
    private RectTransform canvasRect;

    [SerializeField]
    private RectTransform tooltipRoot;

    [SerializeField]
    private LayoutGroup layoutGroup;

    private readonly List<ToolTipPanel> activeTooltips = new();

    private void OnValidate()
    {
        if (layoutGroup == null)
        {
            layoutGroup = tooltipRoot.GetComponent<LayoutGroup>();
        }
    }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    public void Show(List<string> lines)
    {
        Clear();

        foreach (string line in lines)
        {
            var go = Instantiate(tooltipPrefab, tooltipRoot);
            go.SetText(line);
            activeTooltips.Add(go);
        }

        tooltipRoot.gameObject.SetActive(true);
        this.ExecuteDelayedFrame(1, () => LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRoot));
    }

    public void Hide()
    {
        Clear();
        tooltipRoot.gameObject.SetActive(false);
    }

    private void Clear()
    {
        foreach (var go in activeTooltips)
            Destroy(go.gameObject);
        activeTooltips.Clear();
    }

    private void Update()
    {
        if (!tooltipRoot.gameObject.activeSelf) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Screen center
        Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
        bool right = mousePos.x > screenCenter.x;
        bool top = mousePos.y > screenCenter.y;

        // Set pivot so tooltip grows outwards
        tooltipRoot.pivot = new Vector2(right ? 1f : 0f, top ? 1f : 0f);

        // Update layout alignment based on quadrant
        if (layoutGroup is VerticalLayoutGroup vlg)
        {
            if (top && right) vlg.childAlignment = TextAnchor.UpperRight;
            else if (top && !right) vlg.childAlignment = TextAnchor.UpperLeft;
            else if (!top && right) vlg.childAlignment = TextAnchor.LowerRight;
            else vlg.childAlignment = TextAnchor.LowerLeft;
        }

        // Convert screen point to local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePos,
            null,
            out Vector2 localPoint);

        tooltipRoot.localPosition = localPoint;
    }
}