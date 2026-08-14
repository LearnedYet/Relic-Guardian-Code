using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float basicAttackRange = 2f;
    [SerializeField] private LayerMask hitTargetLayers;

    private PlayerAnimator playerAnimator;
    private PlayerInputReader playerInputReader;
    private PlayerActionController playerActionController;
    private PlayerMovement playerMovement;

    private bool isHitWindowOpen;
    private bool isAttackFacingActive;
    private Collider currentAttackTarget;
    private Collider confirmedAttackTarget;

    public bool IsHitWindowOpen
    {
        get { return isHitWindowOpen; }
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

        if (IsCurrentAttackTargetInRange())
        {
            confirmedAttackTarget = currentAttackTarget;
            EnemyHealth enemyHealth = confirmedAttackTarget.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
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

    private void Update()
    {
        if (playerInputReader.ConsumeAttack()
            && playerActionController.TryStartBasicAttack(playerMovement.IsGrounded))
        {
            currentAttackTarget = FindNearestBasicAttackTarget();
            isAttackFacingActive = currentAttackTarget != null;

            playerAnimator.PlayAttack();
        }

        if (isAttackFacingActive && currentAttackTarget != null)
        {
            Vector3 directionToTarget = currentAttackTarget.bounds.center - transform.position;
            directionToTarget.y = 0f;
            playerMovement.FaceDirection(directionToTarget);
        }
    }

    private Collider[] FindBasicAttackCandidates()
    {
        return Physics.OverlapSphere(transform.position, basicAttackRange, hitTargetLayers);
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
