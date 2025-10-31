using UnityEngine;
using UnityEngine.AI;

public class SpeedWiggleModifier : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] TentacleBehavior[] tentacleBehaviorsToModify;
    [SerializeField] float dividedBy = 10;

    void Update()
    {
        ModifyWiggle(agent.speed);
    }

    void ModifyWiggle(float speed)
    {
        float modifier = speed / dividedBy;
        for (int i = 0; i < tentacleBehaviorsToModify.Length; i++)
        {
            tentacleBehaviorsToModify[i].SetMod_WiggleMagnitudeParameters(modifier);
        }
    }
}