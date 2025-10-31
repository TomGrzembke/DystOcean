using MyBox;
using UnityEngine;
using UnityEngine.AI;

public class StatusManager : MonoBehaviour
{
    public bool IsPlayer => isPlayer;
    [SerializeField] bool isPlayer;

    //Public getter, private setter oneliner
    [field: SerializeField] public Creatures CreatureType { get; private set; }
    public Creatures TargetLayer => targetLayer;
    [SerializeField] Creatures targetLayer;

    [field: SerializeField] public StunSubject StunSunject { get; private set; }
    [field: SerializeField] public HealthSubject HealthSubject { get; private set; }
    [field: SerializeField] public SpeedSubject SpeedSubject { get; private set; }
    [field: SerializeField] public GrabTransManager GrabManager { get; private set; }
    [field: SerializeField] public ApplyStatusEffects ApplyStatusEffects { get; private set; }
    [field: SerializeField, ConditionalField(nameof(isPlayer), true)] public CreatureRewards CreatureRewards { get; private set; }
    [field: SerializeField] public PointSubject PointSubject { get; private set; }
    [field: SerializeField, ConditionalField(nameof(isPlayer))] public NavMeshAgent Agent { get; private set; } 

    public void AddHealth(float additionalHealth)
    {
        HealthSubject.AddHealth(additionalHealth);
    }

    public void AddDamage(float additionalHealth)
    {
        HealthSubject.AddHealth(-additionalHealth);
    }

    public void AddStun(float additionalStun)
    {
        if (Time.timeScale <= 0) return;

        StunSunject.AddStun(additionalStun);
    }

    public void AddSpeedModifier(SpeedModifier speedModifier)
    {
        SpeedSubject.AddSpeedModifier(speedModifier);
    }

    public void RemoveSpeedModifier(SpeedModifier speedModifier)
    {
        SpeedSubject.RemoveSpeedModifier(speedModifier);
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