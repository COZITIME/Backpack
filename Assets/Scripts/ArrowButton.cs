using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ArrowButton : MonoBehaviour
{

    [SerializeField]
    private bool isFaceDirection = true;

    [SerializeField, ShowIf(nameof(isFaceDirection))]
    public FaceDirection faceDirection;
    

    [SerializeField, DisplayAsString]
    private Image image;


    public event Action<FaceDirection?> OnClicked;


    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }


    public FaceDirection? FaceDirection => isFaceDirection ? faceDirection : null;
  


    private void OnClick()
    {
        OnClicked?.Invoke(FaceDirection);
    }

    private void OnValidate()
    {
        image = GetComponent<Image>();
    }

    public void UpdateSprite(FaceDirection? direction, Sprite sprite, Color color)
    {
        if (direction == FaceDirection)
        {
            image.sprite = sprite;
            image.color = color;
        }
    }
    
}

public static class StringUtils
{
    /// <summary>
    /// Returns everything after the first underscore.
    /// If no underscore is found, returns the original string.
    /// </summary>
    public static string AfterFirstUnderscore(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        int idx = text.IndexOf('_');
        if (idx >= 0 && idx < text.Length - 1)
            return text.Substring(idx + 1);

        return text;
    }
}