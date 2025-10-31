using UnityEngine;
using UnityEngine.AI;

public class CreatureAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float speedSmoothing = 5;

    float lastSmoothing;

    void Update()
    {
        var smoothedValue = Mathf.Lerp(lastSmoothing, agent.velocity.magnitude, Time.deltaTime * speedSmoothing);

        anim.SetFloat("speed", smoothedValue);
        lastSmoothing = smoothedValue;
    }
}