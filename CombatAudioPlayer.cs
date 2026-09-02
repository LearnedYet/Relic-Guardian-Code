using UnityEngine;

public sealed class CombatAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources = new AudioSource[0];

    private const double ScheduleLeadTime = 0.02d;

    public void Play(CombatAudioData audioData)
    {
        if (audioData == null)
        {
            return;
        }

        StopAll();

        int layerCount = Mathf.Min(audioData.Layers.Length, audioSources.Length);
        double baseDspTime = AudioSettings.dspTime + ScheduleLeadTime;

        for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
        {
            CombatAudioLayer layer = audioData.Layers[layerIndex];
            AudioSource audioSource = audioSources[layerIndex];

            if (layer == null || audioSource == null || layer.Clip == null)
            {
                continue;
            }

            audioSource.clip = layer.Clip;
            audioSource.volume = audioData.MasterVolume * layer.Volume;
            audioSource.pitch = layer.Pitch;
            audioSource.PlayScheduled(baseDspTime + layer.DelaySeconds);
        }
    }

    private void OnDisable()
    {
        StopAll();
    }

    private void StopAll()
    {
        for (int sourceIndex = 0; sourceIndex < audioSources.Length; sourceIndex++)
        {
            AudioSource audioSource = audioSources[sourceIndex];

            if (audioSource == null)
            {
                continue;
            }

            audioSource.Stop();
        }
    }
}
