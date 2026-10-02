using UnityEngine;

public class EnemyHitPresentation : MonoBehaviour
{
    [SerializeField] private Transform hitImpactAnchor;
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private float hitImpactLifetime = 1.2f;
    [SerializeField] private float hitImpactScale = 0.33f;
    //格挡反击命中特效
    [SerializeField] private GameObject guardCounterHitImpactPrefab;
    [SerializeField] private float guardCounterHitImpactLifetime = 1.2f;
    [SerializeField] private float guardCounterHitImpactScale = 0.33f;
    //攻击命中
    [SerializeField] private CombatAudioPlayer hitAudioPlayerPrefab;
    [SerializeField] private CombatAudioData hitAudioData = new CombatAudioData();
    //格挡反击命中音效数据 按索引排列
    [SerializeField] private CombatAudioData[] guardCounterHitAudioDataByIndex = new CombatAudioData[2];
    [SerializeField] private float hitAudioLifetime = 2.5f;
    [SerializeField] private HitstopController hitstopController;
    [SerializeField] private float hitstopDuration = 0.035f;


    public void PresentHit(HitContext hitContext)
    {
        PlayHitImpact(hitContext);
        PlayHitAudio(hitContext);

        if (hitstopController != null)
        {
            hitstopController.RequestHitstop(hitstopDuration);
        }
    }

    private void PlayHitImpact(HitContext hitContext)
    {
        GameObject selectedImpactPrefab = hitImpactPrefab;
        float selectedImpactLifetime = hitImpactLifetime;
        float selectedImpactScale = hitImpactScale;

        if (hitContext.FeedbackType == HitFeedbackType.GuardCounter && guardCounterHitImpactPrefab != null)
        {
            selectedImpactPrefab = guardCounterHitImpactPrefab;
            selectedImpactLifetime = guardCounterHitImpactLifetime;
            selectedImpactScale = guardCounterHitImpactScale;
        }
        if (hitImpactAnchor == null || selectedImpactPrefab == null)
        {
            return;
        }

        GameObject hitImpactInstance = Instantiate(selectedImpactPrefab, hitImpactAnchor.position, hitImpactAnchor.rotation);

        ParticleSystem[] hitImpactParticleSystems = hitImpactInstance.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem hitImpactParticleSystem in hitImpactParticleSystems)
        {
            hitImpactParticleSystem.transform.localScale *= selectedImpactScale;
        }
        Destroy(hitImpactInstance, selectedImpactLifetime);
    }

    private void PlayHitAudio(HitContext hitContext)
    {
        if (hitAudioPlayerPrefab == null)
        {
            return;
        }

        CombatAudioPlayer hitAudioPlayerInstance = Instantiate(hitAudioPlayerPrefab);
        //赋值给选中的音频数据
        CombatAudioData selectedAudioData = hitAudioData;

        if (hitContext.FeedbackType == HitFeedbackType.GuardCounter)
        {
            int audioIndex = hitContext.HitIndex - 1;

            //防越界 配置缺失的时候 播放错误的音频
            if (guardCounterHitAudioDataByIndex != null
                && audioIndex >= 0
                && audioIndex < guardCounterHitAudioDataByIndex.Length
                && guardCounterHitAudioDataByIndex[audioIndex] != null)
            {
                selectedAudioData = guardCounterHitAudioDataByIndex[audioIndex];
            }
        }

        //创建实例化对象
        hitAudioPlayerInstance.Play(selectedAudioData);

        Destroy(hitAudioPlayerInstance.gameObject, hitAudioLifetime);
    }
}
