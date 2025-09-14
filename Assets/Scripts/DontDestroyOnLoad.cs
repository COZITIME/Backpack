using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DontDestroyOnLoad : MonoBehaviour
{
    private static List<int> spawned = new List<int>();
    
    [SerializeField]
    private int id;

    private void OnValidate()
    {
        if (id == 0)
        {
            id = Random.Range(int.MinValue, int.MaxValue);
        }
    }

    private void Awake()
    {
        if (spawned.Contains(id))
        {
            Destroy(gameObject);
        }
        else
        {
            spawned.Add(id);
            DontDestroyOnLoad(gameObject);
        }
    }
}