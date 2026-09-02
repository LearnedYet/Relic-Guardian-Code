using UnityEngine;

public class PlayerGuardPresentation : MonoBehaviour
{
    [SerializeField] private Transform guardImpactAnchor;
    [SerializeField] private GameObject ordinaryGuardImpactPrefab;
    [SerializeField] private GameObject perfectGuardImpactPrefab;
    [SerializeField] private float ordinaryGuardImpactLifetime = 1.2f;
    [SerializeField] private float perfectGuardImpactLifetime = 1.8f;
    [SerializeField] private CombatAudioPlayer combatAudioPlayer;
    [SerializeField] private CombatAudioData ordinaryGuardAudioData = new CombatAudioData();
    [SerializeField] private CombatAudioData perfectGuardAudioData = new CombatAudioData();

    public void PresentGuardResult(GuardResult guardResult, Vector3 incomingDirection)
    {
        if (guardResult == GuardResult.Perfect)
        {
            PlayPerfectGuardImpact();

            if (combatAudioPlayer != null)
            {
                combatAudioPlayer.Play(perfectGuardAudioData);
            }

            Debug.Log("Perfect Guard");
            return;
        }

        if (guardResult == GuardResult.Ordinary)
        {
            PlayOrdinaryGuardImpact();

            if (combatAudioPlayer != null)
            {
                combatAudioPlayer.Play(ordinaryGuardAudioData);
            }

            Debug.Log("Ordinary Guard");
        }
    }

    private void PlayOrdinaryGuardImpact()
    {
        GameObject ordinaryGuardImpactInstance = Instantiate(
            ordinaryGuardImpactPrefab,
            guardImpactAnchor.position,
            guardImpactAnchor.rotation
        );

        Destroy(
            ordinaryGuardImpactInstance,
            ordinaryGuardImpactLifetime
        );
    }

    private void PlayPerfectGuardImpact()
    {
        GameObject perfectGuardImpactInstance = Instantiate(
            perfectGuardImpactPrefab,
            guardImpactAnchor.position,
            guardImpactAnchor.rotation
        );

        Destroy(
            perfectGuardImpactInstance,
            perfectGuardImpactLifetime
        );
    }
}
