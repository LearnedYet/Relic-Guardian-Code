using UnityEngine;

public class HitstopController : MonoBehaviour
{
    private bool isHitstopActive;
    private bool isSlowMotionActive;
    private float timeScaleBeforeEffects;
    private float hitstopEndTime;
    private float slowMotionEndTime;
    private float slowMotionScale;

    public void RequestHitstop(float duration)
    {
        if (!isActiveAndEnabled || duration <= 0f)
        {
            return;
        }

        float requestedEndTime = Time.unscaledTime + duration;

        if (!isHitstopActive && !isSlowMotionActive)
        {
            timeScaleBeforeEffects = Time.timeScale;
        }

        isHitstopActive = true;
        hitstopEndTime = Mathf.Max(hitstopEndTime, requestedEndTime);
        ApplyTimeScale();
    }

    public void RequestSlowMotion(float duration, float scale)
    {
        if (!isActiveAndEnabled || duration <= 0f || scale <= 0f || scale >= 1f)
        {
            return;
        }

        if (!isHitstopActive && !isSlowMotionActive)
        {
            timeScaleBeforeEffects = Time.timeScale;
        }

        if (!isSlowMotionActive)
        {
            slowMotionScale = scale;
        }
        else
        {
            slowMotionScale = Mathf.Min(slowMotionScale, scale);
        }

        isSlowMotionActive = true;
        slowMotionEndTime = Mathf.Max(slowMotionEndTime, Time.unscaledTime + duration);
        ApplyTimeScale();
    }

    private void Update()
    {
        if (!isHitstopActive && !isSlowMotionActive)
        {
            return;
        }

        if (isHitstopActive && Time.unscaledTime >= hitstopEndTime)
        {
            isHitstopActive = false;
            hitstopEndTime = 0f;
        }

        if (isSlowMotionActive && Time.unscaledTime >= slowMotionEndTime)
        {
            isSlowMotionActive = false;
            slowMotionEndTime = 0f;
            slowMotionScale = 0f;
        }

        ApplyTimeScale();
    }

    private void OnDisable()
    {
        RestoreTimeScale();
    }

    private void RestoreTimeScale()
    {
        if (!isHitstopActive && !isSlowMotionActive)
        {
            return;
        }

        isHitstopActive = false;
        isSlowMotionActive = false;
        hitstopEndTime = 0f;
        slowMotionEndTime = 0f;
        slowMotionScale = 0f;
        ApplyTimeScale();
    }

    private void ApplyTimeScale()
    {
        if (isHitstopActive)
        {
            Time.timeScale = 0f;
        }
        else if (isSlowMotionActive)
        {
            Time.timeScale = timeScaleBeforeEffects * slowMotionScale;
        }
        else
        {
            Time.timeScale = timeScaleBeforeEffects;
        }
    }
}
