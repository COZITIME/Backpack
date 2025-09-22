using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicButton : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly]
    private Image image;

    [SerializeField, ChildGameObjectsOnly]
    private TMP_Text text;

    [SerializeField, ChildGameObjectsOnly]
    private Button button;

    private event Action OnClick;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        OnClick?.Invoke();
    }

    public void SetUp(EntityData entityPrefab, Action onComplete = null)
    {
        OnClick = onComplete;
        text.text = entityPrefab.ColouredName + "\n" + entityPrefab.Description;
        image.sprite = entityPrefab.GetComponent<SpriteRenderer>().sprite;
    }
}