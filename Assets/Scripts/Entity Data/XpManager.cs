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

    private int _xp;
    private int _level;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _level = GetLevel();
        levelText.text = $"Lv {_level}";
        xpBar.fillAmount = GetFillAmount();
        UpdateBonePosition(0f);

        GainXp(0);
    }

    public void GainXp(int amount)
    {
        _xp += amount;

        int newLevel = GetLevel();
        float newFill = GetFillAmount();

        // Animate the bar fill
        xpBar.DOFillAmount(newFill, 0.35f)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() => UpdateBonePosition(xpBar.fillAmount));

        // If we leveled up, animate the text & fire the action
        if (newLevel > _level)
        {
            _level = newLevel;

            // Invoke the event so other systems can react
            OnLevelUp?.Invoke(_level);

            // Animate the text
            levelText.text = $"Lv {_level}";
            levelText.transform
                .DOScale(1.3f, 0.15f)
                .SetLoops(2, LoopType.Yoyo);

            SoundManager.Instance.Play(levelUpSound);

            RelicSpawnManager.Instance.SpawnRelics(() => { Debug.Log("Spawn Relics"); });
        }
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

    public int GetLevel()
    {
        int level = 1;
        int xpForNext = xpRequiredAt1;
        int remainingXp = _xp;

        while (remainingXp >= xpForNext)
        {
            remainingXp -= xpForNext;
            level++;
            xpForNext += xpIncreaseAtNextLevel;
        }

        return level;
    }

    public int GetXpToNextLevel()
    {
        int xpForNext = xpRequiredAt1;
        int totalXpNeeded = 0;

        while (_xp >= totalXpNeeded + xpForNext)
        {
            totalXpNeeded += xpForNext;
            xpForNext += xpIncreaseAtNextLevel;
        }

        return (totalXpNeeded + xpForNext) - _xp;
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