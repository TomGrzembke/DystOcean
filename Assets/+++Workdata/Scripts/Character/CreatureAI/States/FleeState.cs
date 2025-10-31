using MyBox;
using UnityEngine;
using UnityEngine.AI;

public class FleeState : State
{
    #region serialized fields
    [MinMaxRange(0, 10)]
    [SerializeField] RangedFloat randomMoveLength = new(0, 10);
    [SerializeField] StatusManager nearestStatusTarget;

    #endregion

    #region private fields
    NavMeshAgent agent => creatureLogic.agent;
    #endregion

    public override State SwitchStateInternal()
    {
        return this;
    }

    protected override void EnterInternal()
    {
    }

    protected override void ExitInternal()
    {
    }

    protected override void FixedUpdateInternal()
    {
        creatureLogic.HandleRotate();
    }

    protected override void UpdateInternal()
    {
        creatureLogic.HandleDetection();

        if (creatureLogic.ClosestManagerTarget && agent.velocity.magnitude > 1f)
        {
            creatureLogic.SetDestination(transform.position + (transform.position - creatureLogic.ClosestManagerTarget.GetPosition()));
        }
        else
        {
            if (agent.remainingDistance > 5) return;
            int pathHorizontal = Random.Range(-1, 2);
            int pathVertical = Random.Range(-1, 2);
            Vector3 pathAddVec3 = new(pathVertical, pathHorizontal);

            float randomMultiplier = Random.Range(randomMoveLength.Min, randomMoveLength.Max);
            creatureLogic.SetDestination(transform.position + pathAddVec3 * randomMultiplier);
        }
    }
}