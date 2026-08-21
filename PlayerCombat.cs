using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerAttackData[] attacks;
    [SerializeField] private LayerMask hitTargetLayers;

    private PlayerAnimator playerAnimator;
    private PlayerInputReader playerInputReader;
    private PlayerActionController playerActionController;
    private PlayerMovement playerMovement;

    private bool isHitWindowOpen;
    private bool isAttackFacingActive;
    private bool isBasicAttackLungeActive;
    private float basicAttackLungeDistanceTraveled;
    private bool isComboWindowOpen;
    private Collider currentAttackTarget;
    private Collider confirmedAttackTarget;
    private int currentAttackIndex;

    public bool IsHitWindowOpen
    {
        get { return isHitWindowOpen; }
    }

    private PlayerAttackData CurrentAttackData
    {
        get { return attacks[currentAttackIndex]; }
    }

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInputReader = GetComponent<PlayerInputReader>();
        playerActionController = GetComponent<PlayerActionController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OpenHitWindow()
    {
        isHitWindowOpen = true;
        isAttackFacingActive = false;
        isBasicAttackLungeActive = false;

        if (IsCurrentAttackTargetInRange())
        {
            confirmedAttackTarget = currentAttackTarget;
            EnemyHealth enemyHealth = confirmedAttackTarget.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(CurrentAttackData.Damage);
            }
        }
        else
        {
            confirmedAttackTarget = null;
        }
    }

    public void CloseHitWindow()
    {
        isHitWindowOpen = false;
        confirmedAttackTarget = null;
    }

    public void OpenComboWindow()
    {
        isComboWindowOpen = true;
    }

    public void CloseComboWindow()
    {
        isComboWindowOpen = false;
    }

    private void Update()
    {
        if (playerInputReader.ConsumeAttack()
            && playerActionController.TryStartBasicAttack(playerMovement.IsGrounded))
        {
            StartAttackStep(0);
        }

        if (isAttackFacingActive && currentAttackTarget != null)
        {
            Vector3 directionToTarget = currentAttackTarget.bounds.center - transform.position;
            directionToTarget.y = 0f;
            playerMovement.FaceDirection(directionToTarget);

            if (isBasicAttackLungeActive)
            {
                float requestedMoveDistance = CurrentAttackData.LungeSpeed * Time.deltaTime;
                float remainingDistance = CurrentAttackData.LungeDistance - basicAttackLungeDistanceTraveled;
                float moveDistance = Mathf.Min(requestedMoveDistance, remainingDistance);
                playerMovement.MoveDuringAttack(directionToTarget, moveDistance);
                basicAttackLungeDistanceTraveled += moveDistance;

                if (basicAttackLungeDistanceTraveled >= CurrentAttackData.LungeDistance)
                {
                    isBasicAttackLungeActive = false;
                }
            }
        }
    }

    private void StartAttackStep(int attackIndex)
    {
        currentAttackIndex = attackIndex;
        currentAttackTarget = FindNearestBasicAttackTarget();
        isAttackFacingActive = currentAttackTarget != null;
        basicAttackLungeDistanceTraveled = 0f;
        isBasicAttackLungeActive = currentAttackTarget != null;

        playerAnimator.PlayAttack();
    }

    private Collider[] FindBasicAttackCandidates()
    {
        return Physics.OverlapSphere(
            transform.position,
            CurrentAttackData.TargetRange,
            hitTargetLayers
        );
    }

    private bool IsCurrentAttackTargetInRange()
    {
        if (currentAttackTarget == null)
        {
            return false;
        }

        Collider[] candidates = FindBasicAttackCandidates();
        foreach (Collider candidate in candidates)
        {
            if (candidate == currentAttackTarget)
            {
                return true;
            }
        }

        return false;
    }

    private Collider FindNearestBasicAttackTarget()
    {
        Collider[] candidates = FindBasicAttackCandidates();
        Collider nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider candidate in candidates)
        {
            Vector3 directionToCandidate = candidate.bounds.center - transform.position;
            directionToCandidate.y = 0f;
            float distanceToCandidate = directionToCandidate.magnitude;

            if (distanceToCandidate < nearestDistance)
            {
                nearestDistance = distanceToCandidate;
                nearestTarget = candidate;
            }
        }

        return nearestTarget;
    }
}
