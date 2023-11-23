using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class CreatureLogic : MonoBehaviour
{
    #region serialized fields
    public HealthSubject TargetHealthScript => targetHealthScript;
    [Header("AI Targeting")]
    [SerializeField] HealthSubject targetHealthScript;

    public LayerMask TargetLayer => targetLayer;
    [SerializeField] LayerMask targetLayer;

    public bool CanSeeTarget => canSeeTarget;
    [SerializeField] bool canSeeTarget = false;

    [SerializeField] float distanceFromTarget;
    public float DistanceFromTarget => distanceFromTarget;

    [Header("AI Settings")]
    [SerializeField] float defaultAgentSpeed = 3.5f;

    [SerializeField] float defaultAgentAcceleration = 3.5f;

    [SerializeField] float defaultAgentStoppingDistance = 3.5f;

    [SerializeField] float detectionRadius;
    public float DetectionRadius => detectionRadius;

    public float DetectionAngle => detectionAngle;
    [Range(0, 360)][SerializeField] float detectionAngle = 50f;
    public LayerMask ObstacleLayer => obstacleLayer;
    [SerializeField] LayerMask obstacleLayer;

    [Header("States")]
    [SerializeField] protected StateManager stateManager;
    [Range(0, 1), SerializeField] float fleePercentage = .2f;

    [Header("Map")]
    [SerializeField] SpriteRenderer mapSpriteRenderer;

    [Header("Health")]
    [SerializeField] LimbManager limbManager;
    #endregion

    #region private fields

    protected StunState stunState;
    protected ChaseState chaseState;
    protected FleeState fleeState;
    protected DeathState deathState;
    protected StunSubject stun;
    [HideInInspector] public NavMeshAgent agent;
    HealthSubject healthSubject;
    SpeedSubject speedSubject;

    #endregion

    void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        mapSpriteRenderer = GetSpriteRendererInLayer(gameObject, "Map");
        ResetAgentVars();
    }

    void Update()
    {
        HandleMapIndicators();
    }

    void OnValidate()
    {
        fleeState = GetComponentInChildren<FleeState>();
        deathState = GetComponentInChildren<DeathState>();
        healthSubject = GetComponentInChildren<HealthSubject>();
        speedSubject = GetComponentInChildren<SpeedSubject>();
        stunState = GetComponentInChildren<StunState>();
        chaseState = GetComponentInChildren<ChaseState>();
        stun = GetComponentInChildren<StunSubject>();
        agent = GetComponent<NavMeshAgent>();

        ResetAgentVars();
    }
    void OnEnable()
    {
        stun.RegisterOnStun(OnStun);
        healthSubject.RegisterOnHealthChangedAlpha(OnHealthChangedAlpha);
    }
    void OnDisable()
    {
        stun.OnStun -= OnStun;
        healthSubject.OnHealthChangedAlpha -= OnHealthChangedAlpha;
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

            float step = agent.acceleration * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        }
    }

    public SpriteRenderer GetSpriteRendererInLayer(GameObject parent, string layerName)
    {
        SpriteRenderer spriteRendererInLayer = null;

        // Get all child objects of the parent GameObject
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);

        // Find the first child object that matches the layer, is active, and has a SpriteRenderer component
        foreach (Transform child in allChildren)
        {
            SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            if (childSpriteRenderer != null && child.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                spriteRendererInLayer = childSpriteRenderer;
                break;
            }
        }

        return spriteRendererInLayer;
    }

    public void HandleMapIndicators()
    {
        if (stateManager.currentState == chaseState)
            if (mapSpriteRenderer != null)
                mapSpriteRenderer.color = new Color(255, 0, 0);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, DetectionRadius); //This visualizes the detection radius

        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -detectionAngle / 2); //This seperates the Angle into two different values
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, detectionAngle / 2); //This seperates the Angle into two different values

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

    void OnHealthChangedAlpha(float alpha)
    {
        if (alpha <= 0)
        {
            stateManager.SetState(deathState);
            return;
        }

        if (alpha <= fleePercentage)
        {
            stateManager.SetState(fleeState);
            return;
        }

        stateManager.SetState(stateManager.LastState);
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

    public void SetTargetHealthScript(HealthSubject newTarget)
    {
        if (newTarget != healthSubject)
            targetHealthScript = newTarget;
    }
    #endregion
}