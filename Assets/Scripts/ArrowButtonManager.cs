using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowButtonManager : MonoBehaviour
{
    public static ArrowButtonManager Instance { get; private set; }

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite[] disabledSprites;

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite[] biteSprites;

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite[] walkSprites;

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite[] vomitSprites;

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite chewEatSprite;

    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private Sprite chewNormalSprite;


    [SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private FaceDirection[] directions;

    [BoxGroup("Buttons"), SerializeField]
    private Color disabledColor;

    [BoxGroup("Buttons"), SerializeField]
    private Color walkColor;

    [BoxGroup("Buttons"), SerializeField]
    private Color biteColor;

    [BoxGroup("Buttons"), SerializeField, ListDrawerSettings(ShowIndexLabels = true, DefaultExpandedState = true)]
    private ArrowButton[] arrowButtons;

    [BoxGroup("Buttons"), SerializeField]
    private ArrowButton chewButton;

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void UpdateButtons()
    {
        var canWalkToList = MouthHelper.GetWalkDirections();
        var canEatList = MouthHelper.GetEatDirections();

        for (int i = 0; i < directions.Length; i++)
        {
            var index = i;
            var direction = directions[index];

            foreach (var arrow in arrowButtons)
            {
                bool canWalkTo = canWalkToList.Contains(direction);
                bool canEat = canEatList.Contains(direction);
                GetSheetAndColour(out var sheet, out var colour, canWalkTo, canEat);
                arrow.UpdateSprite(direction, sheet[index], colour);
            }
        }

        bool hasMorsel = BellyManager.Instance.PlayerBelly.HasMorsel;
        Color mouthColour = hasMorsel ? walkColor : biteColor;
        Sprite mouthSprite = hasMorsel ? chewEatSprite : chewNormalSprite;
        chewButton.UpdateSprite(null, mouthSprite, mouthColour);
    }

    private void Start()
    {
        foreach (var button in arrowButtons)
        {
            button.OnClicked += OnButtonClick;
        }

        UpdateButtons();
    }

    private void OnButtonClick(FaceDirection? direction)
    {
        PlayerInputHandler.Instance.OnButtonClicked(direction);
    }

    void GetSheetAndColour(out Sprite[] sheet, out Color colour, bool canWalkTo, bool canEat)
    {
        if (canEat)
        {
            colour = biteColor;
            sheet = biteSprites;
            return;
        }

        if (canWalkTo)
        {
            colour = walkColor;
            sheet = BellyManager.Instance.PlayerBelly.IsEmpty ? walkSprites : vomitSprites;
            return;
        }

        colour = disabledColor;
        sheet = disabledSprites;
    }


    private void OnValidate()
    {
        arrowButtons = GetComponentsInChildren<ArrowButton>();

        // directions = new FaceDirection[arrowButtons.Length];
        // for (var index = 0; index < arrowButtons.Length; index++)
        // {
        //     var button = arrowButtons[index];
        //     if (button.FaceDirection.HasValue)
        //     {
        //         directions[index] = button.FaceDirection.Value;
        //     }
        // }
    }
}