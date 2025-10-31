using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    public enum ControlState
    {
        PLAYER_CONTROL = 0,
        GAME_CONTROL = 10,
        STUN = 20,
    }

    [Header("References")]
    [SerializeField] SpeedSubject speedSubject;
    [SerializeField] StunSubject stunSubject;

    [Header("Values")]
    [SerializeField] float sprintSpeed = 5;
    [SerializeField] float rotationSmoothing = 10;
    [SerializeField] float timeUntilMaximumSpeed = 1;
    
    [Header("Runtime")]
    [SerializeField] ControlState controlState;
    [SerializeField] bool isPerformingMove;

    private float TARGET_DISTANCE = 5;
    PlayerInputActions inputActions;
    NavMeshAgent agent;
    float currentAgentSpeed;
    Vector2 movement;
    Vector2 moveSafe;

    void Awake()
    {
        inputActions = new();

        inputActions.Player.Move.performed += ctx => Movement(ctx.ReadValue<Vector2>());
        inputActions.Player.Move.canceled += ctx => Movement(ctx.ReadValue<Vector2>());
        inputActions.Player.Sprint.performed += ctx => Sprint(true);
        inputActions.Player.Sprint.canceled += ctx => Sprint(false);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speedSubject.Speed;

        currentAgentSpeed = speedSubject.Speed;
    }

    public void OnEnable()
    {
        inputActions.Enable();
        stunSubject.RegisterOnStun(StunLogic);
    }

    public void OnDisable()
    {
        inputActions.Disable();
        stunSubject.OnStun -= StunLogic;
    }

    void FixedUpdate()
    {
        HandleRotation();
        if (controlState != ControlState.PLAYER_CONTROL) return;

        SmoothSpeed();
        SetAgentPosition();
    }

    void HandleRotation()
    {
        if (agent.velocity == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, agent.velocity.normalized);
        float step = rotationSmoothing * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);
    }

    void SmoothSpeed()
    {
        if (isPerformingMove)
        {
            currentAgentSpeed += SpeedRamp();
        }
        else
        {
            currentAgentSpeed -= SpeedRamp();
        }

        currentAgentSpeed = Mathf.Clamp(currentAgentSpeed, 0, speedSubject.Speed);
        agent.speed = currentAgentSpeed;
    }

    float SpeedRamp()
    {
        return Time.deltaTime * speedSubject.Speed / timeUntilMaximumSpeed;
    }


    void Movement(Vector2 direction)
    {
        isPerformingMove = direction != Vector2.zero;

        if (!isPerformingMove) return;

        movement = direction.normalized;
    }

    public void SetAgentPosition()
    {
        if (movement == Vector2.zero) return;

        moveSafe = Vector2.Lerp(moveSafe, movement, Time.deltaTime * rotationSmoothing);
        SetAgentPosition(transform.position + new Vector3(moveSafe.x, moveSafe.y, 0) * TARGET_DISTANCE);
    }

    public void SetAgentPosition(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    void Sprint(bool condition)
    {
        if (condition)
        {
            speedSubject.AddSpeedModifier(sprintSpeed);
        }
        else
        {
            speedSubject.RemoveSpeedModifier(sprintSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
    }


    public void SetControlState(ControlState newControlState, bool disableAgent = false)
    {
        controlState = newControlState;

        if (disableAgent)
            agent.enabled = !disableAgent;
    }

    public void ReenableMovement(bool enableAgent = false)
    {
        controlState = ControlState.PLAYER_CONTROL;

        if (!enableAgent) return;

        agent.enabled = enableAgent;
    }

    void StunLogic(bool condition)
    {
        if (condition)
        {
            controlState = ControlState.STUN;
        }
        else
        {
            controlState = ControlState.PLAYER_CONTROL;
        }
    }
}