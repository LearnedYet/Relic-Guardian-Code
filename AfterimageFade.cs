using UnityEngine;

public class AfterimageFade : MonoBehaviour
{
    private Material runtimeMaterial;
    private Color startColor;
    private float fadeDuration;
    private float elapsedTime;

    public void Initialize(Material material, float duration)
    {
        runtimeMaterial = material;
        startColor = runtimeMaterial.GetColor("_BaseColor");
        fadeDuration = Mathf.Max(duration, 0.01f);
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (runtimeMaterial == null)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float fadeProgress = Mathf.InverseLerp(0f, fadeDuration, elapsedTime);
        Color currentColor = startColor;
        currentColor.a = Mathf.Lerp(startColor.a, 0f, fadeProgress);

        runtimeMaterial.SetColor("_BaseColor", currentColor);
    }
}
