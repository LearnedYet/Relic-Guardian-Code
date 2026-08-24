using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    [SerializeField] private float lockOnRange = 10f;
    [SerializeField] private float lockOnBreakRange = 12f;
    [SerializeField] private LayerMask lockOnTargetLayers;

    private Collider currentTarget;
    private PlayerInputReader inputReader;

    public Collider CurrentTarget
    {
        get { return currentTarget; }
    }

    public bool IsLockedOn
    {
        get { return currentTarget != null; }
    }

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        if (IsLockedOn && !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
        }

        if (IsLockedOn)
        {
            float distanceToCurrentTarget = Vector3.Distance(transform.position, currentTarget.bounds.center);

            if (distanceToCurrentTarget > lockOnBreakRange)
            {
                currentTarget = null;
            }
        }

        bool lockOnRequested = inputReader.ConsumeLockOn();

        if (lockOnRequested)
        {
            if (IsLockedOn)
            {
                currentTarget = null;
            }
            else
            {
                currentTarget = FindNearestLockOnTarget();
            }
        }
    }

    private Collider[] FindLockOnCandidates()
    {
        return Physics.OverlapSphere(transform.position, lockOnRange, lockOnTargetLayers);
    }

    private Collider FindNearestLockOnTarget()
    {
        Collider[] candidates = FindLockOnCandidates();
        Collider nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider candidate in candidates)
        {
            float distanceToCandidate = Vector3.Distance(transform.position, candidate.bounds.center);

            if (distanceToCandidate < nearestDistance)
            {
                nearestDistance = distanceToCandidate;
                nearestTarget = candidate;
            }
        }

        return nearestTarget;
    }
}
