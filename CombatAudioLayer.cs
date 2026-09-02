using UnityEngine;

[System.Serializable]
public sealed class CombatAudioLayer
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0.25f, 3f)] private float pitch = 1f;
    [SerializeField, Min(0f)] private float delaySeconds = 0;

    public AudioClip Clip
    {
        get { return audioClip; }
    }

    public float Volume
    {
        get { return volume; }
    }

    public float Pitch
    {
        get { return pitch; }
    }

    public float DelaySeconds
    {
        get { return delaySeconds; }
    }
}
