using UnityEngine;

public class PlayerDodgePresentation : MonoBehaviour
{
    [SerializeField] private Material afterimageMaterial;
    [SerializeField, Range(0f, 1f)] private float afterimageStartAlpha = 0.35f;
    [SerializeField] private float afterimageLifetime = 0.35f;
    [SerializeField] private CombatAudioPlayer dodgeStartAudioPlayer;
    [SerializeField] private CombatAudioData dodgeStartAudioData = new CombatAudioData();
    [SerializeField] private CombatAudioPlayer perfectDodgeAudioPlayer;
    [SerializeField] private CombatAudioData perfectDodgeAudioData = new CombatAudioData();
    [SerializeField] private float perfectDodgeSlowMotionDuration;
    [SerializeField] private float perfectDodgeSlowMotionScale;

    private PlayerDodge playerDodge;
    private HitstopController hitstopController;
    private SkinnedMeshRenderer[] playerSkinnedMeshRenderers;
    private MeshRenderer[] playerMeshRenderers;

    private void Awake()
    {
        playerDodge = GetComponent<PlayerDodge>();
        playerSkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        playerMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        hitstopController = GetComponent<HitstopController>();
    }

    public void PresentDodgeStart()
    {
        if (dodgeStartAudioPlayer == null)
        {
            return;
        }

        dodgeStartAudioPlayer.Play(dodgeStartAudioData);
    }

    public void PresentPerfectDodge()
    {
        if (perfectDodgeAudioPlayer != null)
        {
            perfectDodgeAudioPlayer.Play(perfectDodgeAudioData);
        }

        if (hitstopController != null)
        {
            hitstopController.RequestSlowMotion(
                perfectDodgeSlowMotionDuration,
                perfectDodgeSlowMotionScale);
        }

        if (playerDodge == null || afterimageMaterial == null)
        {
            return;
        }

        Quaternion currentToStartRotation =
            playerDodge.DodgeStartRotation * Quaternion.Inverse(transform.rotation);

        if (playerSkinnedMeshRenderers != null)
        {
            foreach (SkinnedMeshRenderer sourceRenderer in playerSkinnedMeshRenderers)
            {
                if (!CanCreateAfterimage(sourceRenderer))
                {
                    continue;
                }

                Mesh afterimageMesh = new Mesh();
                sourceRenderer.BakeMesh(afterimageMesh, false);
                CreateAfterimage(sourceRenderer, afterimageMesh, currentToStartRotation);
            }
        }

        if (playerMeshRenderers == null)
        {
            return;
        }

        foreach (MeshRenderer sourceRenderer in playerMeshRenderers)
        {
            if (!CanCreateAfterimage(sourceRenderer)
                || !IsHighestDetailLodRenderer(sourceRenderer))
            {
                continue;
            }

            MeshFilter sourceMeshFilter = sourceRenderer.GetComponent<MeshFilter>();

            if (sourceMeshFilter == null || sourceMeshFilter.sharedMesh == null)
            {
                continue;
            }

            Mesh afterimageMesh = Instantiate(sourceMeshFilter.sharedMesh);
            CreateAfterimage(sourceRenderer, afterimageMesh, currentToStartRotation);
        }
    }

    private bool CanCreateAfterimage(Renderer sourceRenderer)
    {
        return sourceRenderer != null
            && sourceRenderer.enabled
            && sourceRenderer.gameObject.activeInHierarchy;
    }

    private bool IsHighestDetailLodRenderer(MeshRenderer sourceRenderer)
    {
        LODGroup lodGroup = sourceRenderer.GetComponentInParent<LODGroup>();

        if (lodGroup == null)
        {
            return true;
        }

        LOD[] lods = lodGroup.GetLODs();

        if (lods.Length == 0)
        {
            return true;
        }

        Renderer[] highestDetailRenderers = lods[0].renderers;

        foreach (Renderer highestDetailRenderer in highestDetailRenderers)
        {
            if (highestDetailRenderer == sourceRenderer)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateAfterimage(
        Renderer sourceRenderer,
        Mesh afterimageMesh,
        Quaternion currentToStartRotation)
    {
        GameObject afterimageObject =
            new GameObject($"PerfectDodgeAfterimage_{sourceRenderer.name}");

        MeshFilter afterimageMeshFilter =
            afterimageObject.AddComponent<MeshFilter>();

        MeshRenderer afterimageMeshRenderer =
            afterimageObject.AddComponent<MeshRenderer>();

        afterimageMeshFilter.sharedMesh = afterimageMesh;

        Material runtimeAfterimageMaterial = new Material(afterimageMaterial);
        Color afterimageColor = runtimeAfterimageMaterial.GetColor("_BaseColor");
        afterimageColor.a = afterimageStartAlpha;
        runtimeAfterimageMaterial.SetColor("_BaseColor", afterimageColor);

        Material[] afterimageMaterials =
            new Material[afterimageMesh.subMeshCount];

        for (int materialIndex = 0;
            materialIndex < afterimageMaterials.Length;
            materialIndex++)
        {
            afterimageMaterials[materialIndex] = runtimeAfterimageMaterial;
        }

        afterimageMeshRenderer.sharedMaterials = afterimageMaterials;
        afterimageMeshRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;
        afterimageMeshRenderer.receiveShadows = false;

        AfterimageFade afterimageFade = afterimageObject.AddComponent<AfterimageFade>();
        afterimageFade.Initialize(runtimeAfterimageMaterial, afterimageLifetime);

        Vector3 rendererOffset =
            sourceRenderer.transform.position - transform.position;

        afterimageObject.transform.position =
            playerDodge.DodgeStartPosition
            + currentToStartRotation * rendererOffset;

        afterimageObject.transform.rotation =
            currentToStartRotation * sourceRenderer.transform.rotation;

        afterimageObject.transform.localScale = sourceRenderer.transform.lossyScale;
        afterimageObject.layer = sourceRenderer.gameObject.layer;

        Destroy(afterimageObject, afterimageLifetime);
        Destroy(afterimageMesh, afterimageLifetime);
        Destroy(runtimeAfterimageMaterial, afterimageLifetime);
    }
}
