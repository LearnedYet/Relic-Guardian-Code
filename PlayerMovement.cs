using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private PlayerInputReader inputReader;
    private PlayerActionController playerActionController;
    private PlayerBlock playerBlock;
    private PlayerTargeting playerTargeting;
    private Vector3 currentLocalMoveDirection;
    private float verticalVelocity;
    private float currentSpeed;
    private float currentMovementStrength;
    private bool isJumping;

    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }

    public float CurrentMovementStrength
    {
        get { return currentMovementStrength; }
    }

    public bool IsGrounded
    {
        get { return characterController.isGrounded; }
    }

    public bool IsJumping
    {
        get { return isJumping; }
    }

    public bool IsFalling
    {
        get { return !IsGrounded && verticalVelocity < 0f; }
    }

    public Vector3 CurrentLocalMoveDirection
    {
        get { return currentLocalMoveDirection; }
    }

    public bool CanStartJump
    {
        get
        {
            return playerActionController.CanJump && !playerTargeting.IsLockedOn && characterController.isGrounded;
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
        playerActionController = GetComponent<PlayerActionController>();
        playerBlock = GetComponent<PlayerBlock>();
        playerTargeting = GetComponent<PlayerTargeting>();
    }

    private void Update()
    {
        playerActionController.ResolveActionRequests();

        Vector2 input = inputReader.MoveInput;

        if (!playerActionController.CanMove)
        {
            input = Vector2.zero;
        }

        if (playerActionController.CanSprint && inputReader.IsSprintHeld && playerTargeting.IsLockedOn && input != Vector2.zero)
        {
            playerTargeting.CancelLockOn();
        }

        float selectedMoveSpeed = moveSpeed;

        if (playerActionController.CanSprint && inputReader.IsSprintHeld && !playerTargeting.IsLockedOn)
        {
            selectedMoveSpeed = sprintSpeed;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        Vector3 moveDirection = (cameraRight * input.x + cameraForward * input.y);
        currentLocalMoveDirection = transform.InverseTransformDirection(moveDirection);
        currentSpeed = moveDirection.magnitude * selectedMoveSpeed;
        currentMovementStrength = currentLocalMoveDirection.magnitude;
        verticalVelocity += gravity * Time.deltaTime;

        if (playerActionController.WasJumpAcceptedThisFrame)
        {
            verticalVelocity = jumpSpeed;
            isJumping = true;
        }

        if (playerBlock.IsGuardFacingAssistActive)
        {
            FaceDirection(playerBlock.GuardFacingAssistDirection);
        }
        else if (playerActionController.CanMove && playerTargeting.IsLockedOn)
        {
            Vector3 directionToLockedTarget = playerTargeting.CurrentTarget.bounds.center - transform.position;

            directionToLockedTarget.y = 0f;
            FaceDirection(directionToLockedTarget);
        }
        else if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        Vector3 velocity = moveDirection * selectedMoveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // Small negative value to keep the player grounded
            isJumping = false;
        }
    }

    public void FaceDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void MoveDuringAttack(Vector3 direction, float distance)
    {
        characterController.Move(direction.normalized * distance);
    }
}
