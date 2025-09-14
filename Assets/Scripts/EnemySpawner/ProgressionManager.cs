using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    public event Action<float> OnProgressionChanged;

    private float _progression;

    public int Level => 0;
    
    [ShowInInspector, DisplayAsString]
    public float Progression => _progression;


    private void Awake()
    {
        Instance = this;
    }

    public void OnTurnFinished()
    {
        _progression++;
        OnProgressionChanged?.Invoke(_progression);
    }

    public int GetOtherEntitiesOfKindCount(EntityData entityData)
    {
        // temp using coloured name
        return TurnManager.Instance.GetEntities()
            .Count(x => x.Data.ColouredName == entityData.ColouredName);
    }
}