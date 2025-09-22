using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class RelicSpawnManager : MonoBehaviour
{
    public static RelicSpawnManager Instance { get; private set; }

    [SerializeField]
    private RandomEntityGetter relicGetter;

    [SerializeField]
    private RectTransform levelUpRelicPanel;

    [SerializeField]
    private RelicButton[] relicButtons;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelUpRelicPanel.gameObject.SetActive(false);
        
       
    }


    [Button]
    private void Spawn() => SpawnRelics();

    public void SpawnRelics(Action onRelicSpawned = null)
    {
        PlayerTransform.Instance.GetExtraTurn = true;

        int length = relicButtons.Length;
        var relics = new EntityData[length];
        Time.timeScale = .2f;
        TurnManager.Instance.SetIsPaused(true);
        levelUpRelicPanel.gameObject.SetActive(true);

        List<EntityData> alreadyAdded = new List<EntityData>();
        for (int i = 0; i < length; i++)
        {
            int index = i;

            var nextRelic = relicGetter.GetNewEntity(alreadyAdded);
            alreadyAdded.Add(nextRelic);
            relics[index] = nextRelic;

            relicButtons[i].SetUp(nextRelic, () =>
            {
                var relicToSpawn = relics[index];
                SpawnManager.Instance.Spawn(relicToSpawn);
                Time.timeScale = 1;
                TurnManager.Instance.SetIsPaused(false);
                levelUpRelicPanel.gameObject.SetActive(false);
                onRelicSpawned?.Invoke();
            });
        }
    }
}