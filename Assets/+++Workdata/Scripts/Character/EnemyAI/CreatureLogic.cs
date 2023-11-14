using MyBox;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class CreatureLogic : MonoBehaviour
{
    #region serialized fields
    [Header("AI Settings")]
    [SerializeField] protected StateManager stateManager;
    public Health targetHealthScript;

    public bool CanSeePlayer => canSeePlayer;
    [SerializeField] bool canSeePlayer = false;

    public LayerMask DetectionLayer => detectionLayer;
    [SerializeField] LayerMask detectionLayer;

    public LayerMask ObstacleLayer => obstacleLayer;
    [SerializeField] LayerMask obstacleLayer;

    public float DetectionRadius => detectionRadius;
    [SerializeField] float detectionRadius;

    public float Angle => angle;
    [Range(0, 360)][SerializeField] float angle = 50f;

    [SerializeField] float distanceFromTarget;
    public float DistanceFromTarget => distanceFromTarget;

    public float EnemySpeed => enemySpeed;
    [SerializeField] float enemySpeed = 3.5f;

    public float EnemyAcceleration => enemyAcceleration;
    [SerializeField] float enemyAcceleration = 5f;

    public float EnemyStoppingDistance => enemyStoppingDistance;
    [SerializeField] float enemyStoppingDistance = 1f;

    //animation reference

    [Header("Health Settings")]
    [SerializeField] EnemyLimbStats[] enemyLimbStats;
    #endregion

    #region private fields
    static event Action<CreatureLogic> OnEnemyDied;
    protected StunState stunState;
    protected Stun stun;
    [HideInInspector] public NavMeshAgent agent;
    #endregion

    void Awake()
    {
        stunState = GetComponentInChildren<StunState>();
        stun = GetComponentInChildren<Stun>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    void OnValidate()
    {
        agent = GetComponent<NavMeshAgent>();
        RefreshAgentVars();
    }
    void OnEnable()
    {
        stun.RegisterOnStun(OnStun);
    }
    void OnDisable()
    {
        stun.OnStun -= OnStun;
    }

    public void RefreshAgentVars()
    {
        agent.speed = enemySpeed;
        agent.acceleration = enemyAcceleration;
        agent.stoppingDistance = enemyStoppingDistance;
    }

    public void RefreshAgentVars(float newSpeed = 3, float newAcceleration = 5, float newStoppingDistance = 0)
    {
        enemySpeed = newSpeed;
        enemyAcceleration = newAcceleration;
        enemyStoppingDistance = newStoppingDistance;
        RefreshAgentVars();
    }

    void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, DetectionRadius); //This visualizes the detection radius

        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2); //This seperates the Angle into two different values
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2); //This seperates the Angle into two different values

        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(Vector3.zero, viewAngle01 * DetectionRadius); //This visualizes the FOV 
        Gizmos.DrawLine(Vector3.zero, viewAngle02 * DetectionRadius); //This visualizes the FOV 
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    [ButtonMethod()]
    public int CollectLimbs()
    {
        enemyLimbStats = GetComponentsInChildren<EnemyLimbStats>();
        return enemyLimbStats.Length;
    }

    void OnStun(bool condition)
    {
        if (condition)
        {
            stateManager.SetState(stunState);
            return;
        }

        stateManager.SetState(stateManager.LastState);
        agent.isStopped = condition;
    }

    #region Setter
    public void SetDistanceFromTarget(float newDistance)
    {
        distanceFromTarget = newDistance;
    }

    public void SetCanSeePlayer(bool condition)
    {
        canSeePlayer = condition;
    }
    #endregion
}