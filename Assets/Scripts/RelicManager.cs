using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    private readonly Dictionary<RelicType, int> _relicCounts = new Dictionary<RelicType, int>();

    private void Awake()
    {
        Instance = this;
    }

    public void Eat(RelicType relicType)
    {
        if (_relicCounts.TryGetValue(relicType, out int count))
        {
            _relicCounts[relicType] = count + 1;
        }
        else
        {
            _relicCounts[relicType] = 1;
        }

        OnRelicCountChange(relicType, _relicCounts[relicType]);
    }


    public void Regurgitate(RelicType entityDataRelicType)
    {
        if (_relicCounts.ContainsKey(entityDataRelicType))
        {
            _relicCounts[entityDataRelicType]--;
            _relicCounts[entityDataRelicType] = Mathf.Max(_relicCounts[entityDataRelicType], 0);
            OnRelicCountChange(entityDataRelicType, _relicCounts[entityDataRelicType]);
        }
        else
        {
            OnRelicCountChange(entityDataRelicType, 0);
        }
    }

    public int GetRelicCount(RelicType entityDataRelicType)
    {
        return _relicCounts.TryGetValue(entityDataRelicType, out int count) ? count : 0;
    }

    private void OnRelicCountChange(RelicType relicType, int count)
    {
        if (relicType == RelicType.BellyUp)
        {
            BellyManager.Instance.PlayerBelly.SetBellyBonus (count);
        }
    }
}