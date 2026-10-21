using UnityEngine;

namespace ScrapOfHermes;

/// <summary>An aura manager script</summary>
public class SimpleAura : MonoBehaviour
{
    void Awake()
    {
        SetColor(Aura1, new(0, 0, 0, 0));
        SetColor(Aura2, new(0, 0, 0, 0));
        SetColor(Aura3, new(0, 0, 0, 0));
        SetColor(AuraN3, new(0, 0, 0, 0));
        SetColor(AuraN2, new(0, 0, 0, 0));
        SetColor(AuraN1, new(0, 0, 0, 0));
    }

    /// <summary>Set specify material</summary>
    public void SetMaterial(GameObject target, int idx)
    {
        idx = Mathf.Clamp(idx, 0, samples.Length - 1);

        target.GetComponent<ParticleSystemRenderer>().material = samples[idx];
    }

    /// <summary>Set specify color</summary>
    public void SetColor(GameObject target, Color color)
    {
        var main = target.GetComponent<ParticleSystem>().main;

        main.startColor = color;
    }

    /// <summary>Sample aura 1</summary>
    [SerializeField]
    public GameObject Aura1;

    /// <summary>Sample aura 2</summary>
    [SerializeField]
    public GameObject Aura2;

    /// <summary>Sample aura 3</summary>
    [SerializeField]
    public GameObject Aura3;

    /// <summary>Sample aura n3</summary>
    [SerializeField]
    public GameObject AuraN3;

    /// <summary>Sample aura n2</summary>
    [SerializeField]
    public GameObject AuraN2;

    /// <summary>Sample aura n1</summary>
    [SerializeField]
    public GameObject AuraN1;

    /// <summary>Sample materials</summary>
    [SerializeField]
    public Material[] samples;
}
