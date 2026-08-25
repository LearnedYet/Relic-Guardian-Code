using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private PlayerTargeting playerTargeting;
    [SerializeField] private CinemachineCamera freeLookCamera;
    [SerializeField] private CinemachineCamera lockOnCamera;
    [SerializeField] private Transform lockOnCameraTarget;
    [SerializeField, Range(0f, 1f)] private float enemyLookWeight = 0.35f;

    private CinemachineInputAxisController freeLookInputAxisController;
    private CinemachineInputAxisController lockOnInputAxisController;

    private void Awake()
    {
        freeLookInputAxisController = freeLookCamera.GetComponent<CinemachineInputAxisController>();
        lockOnInputAxisController = lockOnCamera.GetComponent<CinemachineInputAxisController>();
    }

    private void Update()
    {
        if (playerTargeting.IsLockedOn)
        {
            freeLookInputAxisController.enabled = false;
            lockOnInputAxisController.enabled = true;
            Vector3 playerLookPosition = playerTargeting.transform.position + Vector3.up * 1.2f;
            Vector3 enemyLookPosition = playerTargeting.CurrentTarget.bounds.center;
            Vector3 cameraTargetPosition = Vector3.Lerp(playerLookPosition, enemyLookPosition, enemyLookWeight);

            lockOnCameraTarget.position = cameraTargetPosition;

            freeLookCamera.Priority.Value = 0;
            lockOnCamera.Priority.Value = 10;
        }
        else
        {
            freeLookInputAxisController.enabled = true;
            lockOnInputAxisController.enabled = false;
            freeLookCamera.Priority.Value = 10;
            lockOnCamera.Priority.Value = 0;
        }
    }
}
