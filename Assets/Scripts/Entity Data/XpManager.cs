using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // ← Make sure you have DOTween installed

public class XpManager : MonoBehaviour
{
    public static XpManager Instance { get; private set; }

    public event Action<int> OnLevelUp;

    [BoxGroup("UI"), SerializeField]
    private Image xpBar;

    [BoxGroup("UI"), SerializeField]
    private TMP_Text levelText;

    [BoxGroup("UI"), SerializeField]
    private Image bone;

    [BoxGroup("Xp"), SerializeField]
    private int xpRequiredAt1 = 3; // XP to reach level 2

    [BoxGroup("Xp"), SerializeField]
    private int xpIncreaseAtNextLevel = 2; // How much more each level costs


    [SerializeField]
    private AudioClip levelUpSound;


    public int Xp => _xp;
    public int Level => _level;

    private int _xp;
    private int _level;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _level = 1;
        levelText.text = $"Lv {_level}";
        xpBar.fillAmount = GetFillAmount();
        UpdateBonePosition(0f);
        GainXp(0);
    }

    public void GainXp(int amount)
    {
        _xp += amount;


        TryLevelUp();
    }

    private void TryLevelUp()
    {
        float newFill = GetFillAmount();

        bool isLevelUp = IsLevelUp();
        if (isLevelUp) newFill = 1f;

        // Animate the bar fill
        xpBar.DOFillAmount(newFill, 0.35f)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() => UpdateBonePosition(xpBar.fillAmount))
            .SetUpdate(true);

        // If we leveled up, animate the text & fire the action
        if (!isLevelUp) return;
        _level++; // next level

        // Invoke the event so other systems can react
        OnLevelUp?.Invoke(_level);

        // Animate the text
        levelText.text = $"Lv {_level}";
        levelText.transform
            .DOScale(1.3f, 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .SetUpdate(true);

        SoundManager.Instance.Play(levelUpSound);

        RelicSpawnManager.Instance.SpawnRelics(() =>
        {
            Debug.Log("Relic Spawned");
            TryLevelUp();
        });
    }

    private float GetFillAmount()
    {
        int totalXpForCurrentLevel = GetTotalXpForLevel(_level);
        int totalXpForNextLevel = GetTotalXpForLevel(_level + 1);

        float progress = Mathf.InverseLerp(totalXpForCurrentLevel, totalXpForNextLevel, _xp);
        return progress;
    }

    private int GetTotalXpForLevel(int level)
    {
        if (level <= 1) return 0;

        int cost = xpRequiredAt1;
        int total = 0;

        for (int i = 1; i < level; i++)
        {
            total += cost;
            cost += xpIncreaseAtNextLevel;
        }

        return total;
    }

    public bool IsLevelUp() => IsLevelUp(out _);

    public bool IsLevelUp(out int remainingXp)
    {
        int level = 1;
        int xpForNext = xpRequiredAt1;
        remainingXp = _xp;

        while (remainingXp >= xpForNext)
        {
            remainingXp -= xpForNext;
            level++;

            if (level > _level) return true;

            xpForNext += xpIncreaseAtNextLevel;
        }

        return false;
    }


    private void UpdateBonePosition(float fill)
    {
        // We assume the bar uses an Image with "Filled" type = Horizontal
        RectTransform barRect = xpBar.rectTransform;
        RectTransform boneRect = bone.rectTransform;

        // get bar width in local space
        float width = barRect.rect.width;

        // fill is 0..1; anchoredPosition.x is from -width/2 to +width/2 if pivot is 0.5
        float x = (fill - 0.5f) * width;

        Vector2 bonePos = boneRect.anchoredPosition;
        bonePos.x = x;
        boneRect.anchoredPosition = bonePos;
    }
}