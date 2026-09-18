using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDuration = 0.6f;
    [SerializeField] private float dodgeMovementDuration = 0.6f;
    [SerializeField] private float dodgeDistance = 3f;

    private float dodgeEndTime;
    private float dodgeMovementEndTime;
    private PlayerActionController playerActionController;
    private PlayerInputReader playerInputReader;
    private PlayerMovement playerMovement;
    private PlayerTargeting playerTargeting;
    private PlayerAnimator playerAnimator;
    private Vector3 dodgeDirection;
    private float dodgeStartTime;
    private float previousDodgeProgress;

    public Vector3 DodgeDirection => dodgeDirection;

    private void Awake()
    {
        playerActionController = GetComponent<PlayerActionController>();
        playerInputReader = GetComponent<PlayerInputReader>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTargeting = GetComponent<PlayerTargeting>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Dodging)
        {
            return;
        }

        float currentDodgeProgress = Mathf.InverseLerp(dodgeStartTime, dodgeMovementEndTime, Time.time);
        float frameDodgeDistance = (currentDodgeProgress - previousDodgeProgress) * dodgeDistance;
        playerMovement.MoveDuringDodge(dodgeDirection, frameDodgeDistance);
        previousDodgeProgress = currentDodgeProgress;

        if (Time.time >= dodgeEndTime)
        {
            playerActionController.FinishDodge();
            playerAnimator.FinishDodge();
        }
    }

    public void BeginDodge()
    {
        bool hasMoveInput = playerInputReader.MoveInput != Vector2.zero;

        dodgeDirection = GetDodgeDirection();

        if (hasMoveInput && !playerTargeting.IsLockedOn)
        {
            playerMovement.SnapFacing(dodgeDirection);
        }

        playerAnimator.PlayDodge(dodgeDirection, hasMoveInput);
        dodgeStartTime = Time.time;
        dodgeEndTime = dodgeStartTime + Mathf.Max(dodgeDuration, 0.01f);
        dodgeMovementEndTime = Mathf.Min(dodgeStartTime + Mathf.Max(dodgeMovementDuration, 0.01f), dodgeEndTime);
        previousDodgeProgress = 0f;
    }

    private Vector3 GetDodgeDirection()
    {
        Vector2 input = playerInputReader.MoveInput;
        Collider currentTarget = playerTargeting.CurrentTarget;

        if (currentTarget == null)
        {
            return GetFreeDodgeDirection(input);
        }

        return GetLockedDodgeDirection(input, currentTarget);
    }

    private Vector3 GetFreeDodgeDirection(Vector2 input)
    {
        Vector3 direction = playerMovement.GetCameraRelativeDirection(input);

        if (direction == Vector3.zero)
        {
            return -transform.forward;
        }

        return direction.normalized;
    }

    private Vector3 GetLockedDodgeDirection(Vector2 input, Collider target)
    {
        Vector3 targetForward = target.bounds.center - transform.position;
        targetForward.y = 0f;

        if (targetForward == Vector3.zero)
        {
            return -transform.forward;
        }

        targetForward.Normalize();

        if (input == Vector2.zero)
        {
            return -targetForward;
        }

        Vector3 targetRight = Vector3.Cross(Vector3.up, targetForward);

        return (targetRight * input.x + targetForward * input.y).normalized;
    }
}
