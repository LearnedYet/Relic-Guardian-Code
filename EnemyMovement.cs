using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float recoilDistance = 0.15f;
    [SerializeField] private float recoilDuration = 0.12f;

    private Vector3 recoilDirection;
    private float recoilElapsedTime;
    private bool isRecoiling;
    public bool IsRecoiling => isRecoiling;

    private CharacterController characterController;

    public Vector3 CurrentLocalHorizontalVelocity
    {
        get
        {
            Vector3 horizontalVelocity = characterController.velocity;
            horizontalVelocity.y = 0f;
            return transform.InverseTransformDirection(horizontalVelocity);
        }
    }

    public float CurrentHorizontalSpeed => CurrentLocalHorizontalVelocity.magnitude;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void BeginRecoil(Vector3 direction)
    {
        direction.y = 0f;

        if (direction == Vector3.zero || recoilDistance <= 0f || recoilDuration <= 0f)
        {
            return;
        }

        recoilDirection = direction.normalized;
        recoilElapsedTime = 0f;
        isRecoiling = true;
    }

    public void CancelRecoil()
    {
        isRecoiling = false;
        recoilElapsedTime = 0f;
        recoilDirection = Vector3.zero;
    }

    private void OnDisable()
    {
        CancelRecoil();
    }

    private void LateUpdate()
    {
        if (!isRecoiling || Time.deltaTime <= 0f)
        {
            return;
        }

        float previousProgress = recoilElapsedTime / recoilDuration;
        recoilElapsedTime = Mathf.Min(recoilElapsedTime + Time.deltaTime, recoilDuration);
        float progress = recoilElapsedTime / recoilDuration;

        float moveDistance = recoilDistance * ((2f * progress - progress * progress) - (2f * previousProgress - previousProgress * previousProgress));

        characterController.Move(recoilDirection * moveDistance);

        if (recoilElapsedTime >= recoilDuration)
        {
            CancelRecoil();
        }
    }

    public void Turn(Vector3 direction)
    {
        if(isRecoiling || Time.deltaTime <= 0f)
        {
            return;
        }

        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Move(
        Vector3 moveDirection,
        Vector3 facingDirection,
        float speedMultiplier)
    {
        if (isRecoiling || Time.deltaTime <= 0f)
        {
            return;
        }

        moveDirection.y = 0f;
        facingDirection.y = 0f;

        if (moveDirection == Vector3.zero)
        {
            return;
        }

        Turn(facingDirection);

        Vector3 movement =
            moveDirection.normalized *
            moveSpeed *
            Mathf.Max(0f, speedMultiplier) *
            Time.deltaTime;
        characterController.Move(movement);
    }

    public void MoveDuringAttack(Vector3 direction, float distance)
    {
        if (isRecoiling || Time.deltaTime <= 0f || distance <= 0f)
        {
            return;
        }

        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        characterController.Move(direction.normalized * distance);
    }

    public void Stop()
    {
        if (isRecoiling || Time.deltaTime <= 0f)
        {
            return;
        }

        characterController.Move(Vector3.zero);
    }
}
