using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField]
    private SerializedDictionary<ParticleType, ParticleSystem> particleSystemPrefabs = new();

    private void Awake()
    {
        Instance = this;
    }

    public ParticleSystem PlayParticles(ParticleType particleType, Vector2 position)
    {
        var particles = Instantiate(particleSystemPrefabs[particleType], position, Quaternion.identity, transform);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration + .5f);
        return particles;
    }
}