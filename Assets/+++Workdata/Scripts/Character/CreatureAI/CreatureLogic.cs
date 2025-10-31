using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class CreatureLogic : MonoBehaviour
{
    #region serialized fields

    public StatusManager TargetStatusManager => targetStatusManager;

    [Header("AI Targeting")] [SerializeField]
    StatusManager targetStatusManager;

    [SerializeField] StatusManager closestStatusTarget;
    public StatusManager ClosestManagerTarget => closestStatusTarget;
    [SerializeField] float closestDistance;

    public bool CanSeeTarget => canSeeTarget;

    [SerializeField] float distanceFromTarget;
    public float DistanceFromTarget => distanceFromTarget;

    [Header("AI Settings")] [SerializeField]
    float defaultAgentSpeed = 3.5f;

    [SerializeField] float defaultAgentAcceleration = 3.5f;

    [SerializeField] float defaultAgentStoppingDistance = 3.5f;

    [SerializeField] float detectionRadius;
    public float DetectionRadius => detectionRadius;

    [SerializeField] float rotateFactor = 1;

    public float DetectionAngle => detectionAngle;
    [Range(0, 360)] [SerializeField] float detectionAngle = 50f;
    public LayerMask ObstacleLayer => obstacleLayer;
    [SerializeField] LayerMask obstacleLayer;
    public LayerMask CreatureLayer => creatureLayer;
    [SerializeField] LayerMask creatureLayer;

    #endregion

    #region private fields

    bool canSeeTarget = false;
    protected StunState stunState;
    protected ChaseState chaseState;
    protected FleeState fleeState;
    protected DeathState deathState;
    protected StunSubject stun;
    [HideInInspector] public NavMeshAgent agent;
    SpeedSubject speedSubject;
    [SerializeField] StatusManager statusManager;
    [SerializeField] List<StatusManager> statusTargets = new();

    #endregion

    void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SearchChildren();
    }

    void Start()
    {
        ResetAgentVars();
    }

    void SearchChildren()
    {
        fleeState = GetComponentInChildren<FleeState>();
        deathState = GetComponentInChildren<DeathState>();
        speedSubject = GetComponentInChildren<SpeedSubject>();
        stunState = GetComponentInChildren<StunState>();
        chaseState = GetComponentInChildren<ChaseState>();
        stun = GetComponentInChildren<StunSubject>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void ResetAgentVars()
    {
        speedSubject.SetDefault(defaultAgentSpeed);
        agent.acceleration = defaultAgentAcceleration;
        agent.stoppingDistance = defaultAgentStoppingDistance;
    }

    public void SetAgentVars(float newSpeed = -1, float newAcceleration = -1, float newStoppingDistance = -1)
    {
        if (newSpeed != -1)
            speedSubject.SetDefault(newSpeed);
        if (newAcceleration != -1)
            agent.acceleration = newAcceleration;
        if (newStoppingDistance != -1)
            agent.stoppingDistance = newStoppingDistance;
    }

    public void HandleRotate()
    {
        Vector3 velocity = agent.velocity;
        velocity.WithoutZ();
        velocity.Normalize();

        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, velocity);

            float step = agent.acceleration * Time.deltaTime * rotateFactor;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        }
    }

    #region Handle Detection

    public void HandleDetection()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, creatureLayer);

        if (colliders.Length <= 1)
        {
            ResetStatusTarget();
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].TryGetComponent(out StatusManager statusTarget))
                statusTarget = colliders[i].GetComponentInChildren<StatusManager>();
            if (!statusTarget)
                statusTarget = colliders[i].GetComponentInParent<StatusManager>();
            if (!statusTarget)
                statusTarget = colliders[i].transform.parent.GetComponentInParent<StatusManager>();

            if (!statusTarget || statusTarget == statusManager)
                continue;

            if (!statusManager.TargetLayer.HasFlag(statusTarget.CreatureType))
                continue;

            if (!statusTargets.Contains(statusTarget))
                statusTargets.Add(statusTarget);

            CalculateClosestDistance(statusTarget);
            LookLogic(statusTarget);
        }
    }

    void LookLogic(StatusManager targetStatusManager)
    {
        Vector2 targetDirection = targetStatusManager.GetPosition() - transform.position;
        targetDirection = targetDirection.normalized;

        if (Vector2.Angle(transform.up, targetDirection) < detectionAngle / 2)
        {
            if (!Physics2D.Raycast(transform.position, targetDirection, distanceFromTarget, obstacleLayer))
            {
                SetCanSeePlayer(true);
                SetTargetStatusManager(targetStatusManager);
            }
            else
            {
                SetCanSeePlayer(targetStatusManager);
                SetTargetStatusManager(null);
            }
        }
        else if (canSeeTarget)
            SetCanSeePlayer(false);
    }

    void ResetStatusTarget()
    {
        statusTargets.Clear();
        closestStatusTarget = null;
        closestDistance = 100;
    }

    void CalculateClosestDistance(StatusManager statusTarget)
    {
        float dangerDistance = Vector3.Distance(transform.position, statusTarget.transform.position);
        if (dangerDistance < closestDistance || closestStatusTarget == statusTarget)
        {
            closestDistance = dangerDistance;
            closestStatusTarget = statusTarget;
        }
    }

    #endregion

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360,
            DetectionRadius); //This visualizes the detection radius

        Vector3 viewAngle01 =
            DirectionFromAngle(transform.eulerAngles.y,
                -detectionAngle / 2); //This seperates the Angle into two different values
        Vector3 viewAngle02 =
            DirectionFromAngle(transform.eulerAngles.y,
                detectionAngle / 2); //This seperates the Angle into two different values

        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(Vector3.zero, viewAngle01 * DetectionRadius); //This visualizes the FOV 
        Gizmos.DrawLine(Vector3.zero, viewAngle02 * DetectionRadius); //This visualizes the FOV 
    }
#endif

    Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    #region Setter

    public void SetDistanceFromTarget(float newDistance)
    {
        distanceFromTarget = newDistance;
    }

    public void SetCanSeePlayer(bool condition)
    {
        canSeeTarget = condition;
    }

    public void SetTargetStatusManager(StatusManager newTarget)
    {
        if (newTarget != targetStatusManager)
            targetStatusManager = newTarget;
    }

    public void SetDestination(Transform targetTrans)
    {
        agent.SetDestination(targetTrans.position);
    }

    public void SetDestination(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    #endregion
}