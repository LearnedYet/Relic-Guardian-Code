using UnityEngine;

public class EnemyHitPresentation : MonoBehaviour
{
    [SerializeField] private Transform hitImpactAnchor;
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private float hitImpactLifetime = 1.2f;
    [SerializeField] private float hitImpactScale = 0.33f;
    [SerializeField] private CombatAudioPlayer hitAudioPlayerPrefab;
    [SerializeField] private CombatAudioData hitAudioData = new CombatAudioData();
    [SerializeField] private float hitAudioLifetime = 2.5f;
    [SerializeField] private HitstopController hitstopController;
    [SerializeField] private float hitstopDuration = 0.035f;


    public void PresentHit()
    {
        PlayHitImpact();
        PlayHitAudio();

        if (hitstopController != null)
        {
            hitstopController.RequestHitstop(hitstopDuration);
        }
    }

    private void PlayHitImpact()
    {
        if (hitImpactAnchor == null || hitImpactPrefab == null)
        {
            return;
        }

        GameObject hitImpactInstance = Instantiate(hitImpactPrefab, hitImpactAnchor.position, hitImpactAnchor.rotation);

        ParticleSystem[] hitImpactParticleSystems = hitImpactInstance.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem hitImpactParticleSystem in hitImpactParticleSystems)
        {
            hitImpactParticleSystem.transform.localScale *= hitImpactScale;
        }
        Destroy(hitImpactInstance, hitImpactLifetime);
    }

    private void PlayHitAudio()
    {
        if (hitAudioPlayerPrefab == null)
        {
            return;
        }

        CombatAudioPlayer hitAudioPlayerInstance = Instantiate(hitAudioPlayerPrefab);

        hitAudioPlayerInstance.Play(hitAudioData);

        Destroy(hitAudioPlayerInstance.gameObject, hitAudioLifetime);
    }
}
