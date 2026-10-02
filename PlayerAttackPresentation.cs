using UnityEngine;
using UnityEngine.VFX;

public class PlayerAttackPresentation : MonoBehaviour
{
    [SerializeField] private VisualEffect attackTrail;
    [SerializeField] private VisualEffect counterTrail;
    [SerializeField] private CombatAudioPlayer attackAudioPlayer;
    [SerializeField] private CombatAudioData[] attackWhooshAudioDataByIndex = new CombatAudioData[0];
    [SerializeField] private CombatAudioData[] attackWindupAudioDataByIndex = new CombatAudioData[0];
    [SerializeField] private CombatAudioData[] guardCounterWhooshAudioDataByIndex = new CombatAudioData[2];

    public void OpenWeaponTrail()
    {
        SetTrailActive(attackTrail, true);
    }

    public void CloseWeaponTrail()
    {
        SetTrailActive(attackTrail, false);
    }

    public void OpenCounterWeaponTrail()
    {
        SetTrailActive(counterTrail, true);
    }

    public void CloseCounterWeaponTrail()
    {
        SetTrailActive(counterTrail, false);
    }

    private void SetTrailActive(VisualEffect trail, bool isActive)
    {
        if (trail == null)
        {
            return;
        }

        trail.SetBool("Effect Active", isActive);
        trail.SetFloat("Effect Value", isActive ? 1f : 0f);

        if (isActive)
        {
            trail.Play();
        }
        else
        {
            trail.Stop();
        }
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

    public void PlayGuardCounterWhoosh(int swingIndex)
    {
        if (attackAudioPlayer == null
            || guardCounterWhooshAudioDataByIndex == null
            || swingIndex < 0
            || swingIndex >= guardCounterWhooshAudioDataByIndex.Length)
        {
            return;
        }

        attackAudioPlayer.Play(guardCounterWhooshAudioDataByIndex[swingIndex]);
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
        CloseCounterWeaponTrail();
    }

    private void OnDisable()
    {
        CloseWeaponTrail();
        CloseCounterWeaponTrail();
    }
}
