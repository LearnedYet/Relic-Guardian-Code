using UnityEngine;

[System.Serializable]
public sealed class CombatAudioData
{
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] private CombatAudioLayer[] layers = new CombatAudioLayer[0];

    public float MasterVolume
    {
        get { return masterVolume; }
    }

    public CombatAudioLayer[] Layers
    {
        get { return layers; }
    }
}
