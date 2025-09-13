using UnityEngine;

[System.Serializable]
public class EntitySpawn
{
    [SerializeField]
    private EntityData prefab;

    [SerializeField, Min(0)]
    private int minLevelToAppear = 0;

    [SerializeField, Min(0f)]
    private float minProgressionToAppear = 0f;

    [SerializeField, Min(1f)]
    public int maxOfTypeAtOnce = 10;

    public EntityData Prefab => prefab;
    
    public bool IsValid()
    {
        var progressionManager = ProgressionManager.Instance;
        if (minLevelToAppear > progressionManager.Level)
        {
            return false;
        }

        if (minProgressionToAppear > progressionManager.Progression)
        {
            return false;
        }

        var otherEntitiesOfKindCount = progressionManager.GetOtherEntitiesOfKindCount(prefab);
        if (otherEntitiesOfKindCount > maxOfTypeAtOnce)
        {
            return false;
        }

        return true;
    }
}