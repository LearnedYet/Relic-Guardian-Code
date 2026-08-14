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

    public bool TryStartBasicAttack(bool isGrounded)
    {
        if (currentActionState == PlayerActionState.Free && isGrounded)
        {
            currentActionState = PlayerActionState.BasicAttack;
            return true;
        }

        return false;
    }

    public void FinishBasicAttack()
    {
        if (currentActionState == PlayerActionState.BasicAttack)
        {
            currentActionState = PlayerActionState.Free;
        }
    }
}
