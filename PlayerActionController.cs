using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerActionState currentActionState;

    public PlayerActionState CurrentActionState
    {
        get { return currentActionState; }
    }

    public bool CanMove
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    public bool CanJump
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    public bool TryStartAttack(bool isGrounded)
    {
        if (currentActionState == PlayerActionState.Free && isGrounded)
        {
            currentActionState = PlayerActionState.Attacking;
            return true;
        }

        return false;
    }

    public void FinishAttack()
    {
        if (currentActionState == PlayerActionState.Attacking)
        {
            currentActionState = PlayerActionState.Free;
        }
    }
}
