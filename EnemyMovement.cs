using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private CharacterController characterController;

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

        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        characterController.Move(movement);
    }
}
