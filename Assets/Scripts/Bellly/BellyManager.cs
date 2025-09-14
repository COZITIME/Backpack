using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BellyManager : MonoBehaviour
{
    public static BellyManager Instance { get; private set; }
    public Belly PlayerBelly;

    [SerializeField]
    private int bellySize = 10;

    [SerializeField]
    private TMP_Text bellyText;

    [SerializeField]
    private Transform bellyTop;

    private void Awake()
    {
        PlayerBelly = new Belly(bellySize, 1f, bellyTop);
        Instance = this;

        PlayerBelly.OnBellyRepositioning += OnBellyReposition;

        SetText(0);
    }

    private void OnBellyReposition(List<EntityTransform> obj)
    {
        int count = obj.Count;
        SetText(count);
    }

    public void SetText(int amountInBelly)
    {
        var max = PlayerBelly.BellySize;
        if (amountInBelly >= max)
        {
            bellyText.text = "FULL";
            return;
        }

        bellyText.text = $"Capacity: {amountInBelly}/{max}";
    }
}