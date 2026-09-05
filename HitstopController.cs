using UnityEngine;

public class HitstopController : MonoBehaviour
{
    private bool isHitstopActive;
    private float timeScaleBeforeHitstop;
    private float hitstopEndTime;

    public void RequestHitstop(float duration)
    {
        if (!isActiveAndEnabled || duration <= 0f)
        {
            return;
        }

        float requestedEndTime = Time.unscaledTime + duration;

        if (!isHitstopActive)
        {
            timeScaleBeforeHitstop = Time.timeScale;
            isHitstopActive = true;
            Time.timeScale = 0f;
        }

        hitstopEndTime = Mathf.Max(hitstopEndTime, requestedEndTime);
    }

    private void Update()
    {
        if (!isHitstopActive)
        {
            return;
        }

        if (Time.unscaledTime < hitstopEndTime)
        {
            return;
        }

        RestoreTimeScale();
    }

    private void OnDisable()
    {
        RestoreTimeScale();
    }

    private void RestoreTimeScale()
    {
        if (!isHitstopActive)
        {
            return;
        }

        Time.timeScale = timeScaleBeforeHitstop;
        isHitstopActive = false;
        hitstopEndTime = 0f;
    }
}
