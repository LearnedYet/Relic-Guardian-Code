using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private PlayerInputReader inputReader;
    private PlayerActionController playerActionController;
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

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
        playerActionController = GetComponent<PlayerActionController>();
    }

    private void Update()
    {
        Vector2 input = inputReader.MoveInput;

        if (!playerActionController.CanMove)
        {
            input = Vector2.zero;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        bool jumpRequested = inputReader.ConsumeJump();
        Vector3 moveDirection = (cameraRight * input.x + cameraForward * input.y);
        currentSpeed = moveDirection.magnitude * moveSpeed;
        currentMovementStrength = moveDirection.magnitude;
        verticalVelocity += gravity * Time.deltaTime;
        if (jumpRequested && playerActionController.CanJump && characterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
            isJumping = true;
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        Vector3 velocity = moveDirection * moveSpeed;
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
}
