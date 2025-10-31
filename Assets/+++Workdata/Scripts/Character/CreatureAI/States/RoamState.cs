using System.Collections.Generic;
using UnityEngine;

public class RoamState : State
{
    #region serialized fields
    [Header(nameof(RoamState))]
    [SerializeField] float minRoamRange = 5f;
    [SerializeField] float maxRoamRange = 5f;
    [SerializeField] List<StatusManager> statusTargets = new();
    [SerializeField] StatusManager statusManager;
    [SerializeField] State chaseState;
    [SerializeField] bool canRotate = true;
    #endregion

    #region private fields
    Vector2 roamPosition;
    Vector3 startingPosition;
    #endregion

    public override State SwitchStateInternal()
    {
        if (creatureLogic.TargetStatusManager != null)
            return chaseState;
        else
            return this;
    }

    protected override void EnterInternal()
    {
    }

    protected override void UpdateInternal()
    {
        HandleRoaming();
        if(canRotate) creatureLogic.HandleRotate();
        creatureLogic.HandleDetection();
    }

    protected override void FixedUpdateInternal()
    {
    }

    protected override void ExitInternal()
    {
    }

    void Start()
    {
        startingPosition = transform.position;
        roamPosition = startingPosition;
    }


    void HandleRoaming()
    {
        creatureLogic.agent.SetDestination(roamPosition);

        if (Vector3.Distance(transform.position, roamPosition) < stateAgentStoppingDistance + 2)
            roamPosition = GetRandomRoamingPosition();

    }

    Vector3 GetRandomRoamingPosition()
    {
        return startingPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(minRoamRange, maxRoamRange);
    }

    public void HandleDetection()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, creatureLogic.DetectionRadius, creatureLogic.CreatureLayer);

        if (colliders.Length == 0)
        {
            statusTargets.Clear();
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            StatusManager targetStatusManager = colliders[i].GetComponentInChildren<StatusManager>();

            if (statusManager == targetStatusManager)
                continue;
            if (!targetStatusManager)
                continue;

            if (!statusTargets.Contains(targetStatusManager))
                statusTargets.Add(targetStatusManager);

            LookLogic(targetStatusManager);
        }
    }

    void LookLogic(StatusManager targetStatusManager)
    {
        Vector2 targetDirection = (targetStatusManager.GetPosition() - transform.position).normalized;

        if (Vector2.Angle(transform.up, targetDirection) < creatureLogic.DetectionAngle / 2)
        {
            if (!Physics2D.Raycast(transform.position, targetDirection, creatureLogic.DistanceFromTarget, creatureLogic.ObstacleLayer))
            {
                creatureLogic.SetCanSeePlayer(true);
                creatureLogic.SetTargetStatusManager(targetStatusManager);
            }
            else
            {
                creatureLogic.SetCanSeePlayer(false);
                creatureLogic.SetTargetStatusManager(null);
            }
        }
        else if (creatureLogic.CanSeeTarget)
            creatureLogic.SetCanSeePlayer(false);
    }
}