using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController characterController;

    public float CurrentHorizontalSpeed
    {
        get
        {
            Vector3 horizontalVelocity = characterController.velocity;
            horizontalVelocity.y = 0f;
            return horizontalVelocity.magnitude;
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        characterController.Move(movement);
    }

    public void Stop()
    {
        characterController.Move(Vector3.zero);
    }
}
