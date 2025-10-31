using MyBox;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class StatusManager : MonoBehaviour
{
    public bool IsPlayer => isPlayer;
    [SerializeField] bool isPlayer;

    [field: SerializeField] public Creatures CreatureType { get; private set; }


    public Creatures TargetLayer => targetLayer;
    [SerializeField] Creatures targetLayer;

    public StunSubject StunSunject => stunSubject;
    [SerializeField] StunSubject stunSubject;
    public HealthSubject HealthSubject => healthSubject;
    [SerializeField] HealthSubject healthSubject;
    public SpeedSubject SpeedSubject => speedSubject;
    [SerializeField] SpeedSubject speedSubject;
    public GrabTransManager GrabManager => grabManager;
    [SerializeField] GrabTransManager grabManager;
    public ApplyStatusEffects ApplyStatusEffects => applyStatusEffects;
    [SerializeField] ApplyStatusEffects applyStatusEffects;
    public CreatureRewards CreatureRewards => creatureRewards;

    [SerializeField, ConditionalField(nameof(isPlayer), true)]
    CreatureRewards creatureRewards;

    public PointSubject PointSubject => pointSubject;
    [SerializeField] PointSubject pointSubject;

    [SerializeField, ConditionalField(nameof(isPlayer))]
    NavMeshAgent agent;

    public NavMeshAgent Agent => agent;

    public void AddHealth(float additionalHealth)
    {
        healthSubject.AddHealth(additionalHealth);
    }

    public void AddDamage(float additionalHealth)
    {
        healthSubject.AddHealth(-additionalHealth);
    }

    public void AddStun(float additionalStun)
    {
        if (Time.timeScale > 0)
            stunSubject.AddStun(additionalStun);
    }

    public void AddSpeedModifier(SpeedModifier speedModifier)
    {
        speedSubject.AddSpeedModifier(speedModifier);
    }

    public void RemoveSpeedModifier(SpeedModifier speedModifier)
    {
        speedSubject.RemoveSpeedModifier(speedModifier);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}