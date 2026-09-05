using UnityEngine;
using UnityEngine.VFX;

public class PlayerAttackPresentation : MonoBehaviour
{
    [SerializeField] private VisualEffect attackTrail;
    [SerializeField] private CombatAudioPlayer attackAudioPlayer;
    [SerializeField] private CombatAudioData[] attackWhooshAudioDataByIndex = new CombatAudioData[0];
    [SerializeField] private CombatAudioData[] attackWindupAudioDataByIndex = new CombatAudioData[0];

    public void OpenWeaponTrail()
    {
        if (attackTrail == null)
        {
            return;
        }

        attackTrail.SetBool("Effect Active", true);
        attackTrail.SetFloat("Effect Value", 1f);
        attackTrail.Play();
    }

    public void CloseWeaponTrail()
    {
        if (attackTrail == null)
        {
            return;
        }

        attackTrail.SetBool("Effect Active", false);
        attackTrail.SetFloat("Effect Value", 0f);
        attackTrail.Stop();
    }

    public void PlayWeaponWhoosh(int attackIndex)
    {
        if (attackAudioPlayer == null
            || attackWhooshAudioDataByIndex == null
            || attackIndex < 0
            || attackIndex >= attackWhooshAudioDataByIndex.Length)
        {
            return;
        }

        attackAudioPlayer.Play(
            attackWhooshAudioDataByIndex[attackIndex]
        );
    }

    public void PlayWeaponWindup(int attackIndex)
    {
        if (attackAudioPlayer == null
            || attackWindupAudioDataByIndex == null
            || attackIndex < 0
            || attackIndex >= attackWindupAudioDataByIndex.Length)
        {
            return;
        }

        attackAudioPlayer.Play(
            attackWindupAudioDataByIndex[attackIndex]
        );
    }

    private void Awake()
    {
        CloseWeaponTrail();
    }

    private void OnDisable()
    {
        CloseWeaponTrail();
    }
}
