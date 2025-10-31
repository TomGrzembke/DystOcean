using System;
using UnityEngine;

public class ApplyStatusEffects : MonoBehaviour
{
    [SerializeField] bool player;
    [SerializeField] StunEffectCondition stunEffectCondition;

    public void ApplyEffects(StatusEffects _statusEffects, StatusManager targetStatusManager,
        LimbSubject limbSubject = null, StatusManager ownStatusManager = null)
    {
        if (targetStatusManager == ownStatusManager) return;
        if (!targetStatusManager) return;

        bool hasDoneSmth = false;

        if (_statusEffects.damagePerInstance != 0 && limbSubject)
        {
            limbSubject.AddDamage(_statusEffects.damagePerInstance);
            hasDoneSmth = true;
        }

        if (_statusEffects.stunPerInstance != 0)
        {
            if (targetStatusManager != null) 
                if (_statusEffects.stunPerInstance > 0)
                    if (stunEffectCondition != null)
                    {
                        targetStatusManager.AddStun(_statusEffects.stunPerInstance * stunEffectCondition.Calc_percentageDebuff);
                    }
                    else
                    {
                        targetStatusManager.AddStun(_statusEffects.stunPerInstance);
                    }

            hasDoneSmth = true;
        }

        if (_statusEffects.speedModifier)
        {
            targetStatusManager.AddSpeedModifier(_statusEffects.speedModifier);
            hasDoneSmth = true;
        }

        if (!hasDoneSmth) return;

        if (player)
        {
            CombatManager.Instance.CreatureInteraction(targetStatusManager);
        }
        else if (targetStatusManager.IsPlayer)
        {
            CombatManager.Instance.CreatureInteraction(ownStatusManager);
        }
    }

    public void ApplyEffects(StatusEffects _statusEffects, LimbSubject limbSubject,
        StatusManager ownStatusManager = null)
    {
        ApplyEffects(_statusEffects, limbSubject.OwnStatusManager, limbSubject, ownStatusManager);
    }

    public void RemoveSpeedModifier(StatusManager targetStatusManager, SpeedModifier speedModifier)
    {
        if (speedModifier)
            targetStatusManager.RemoveSpeedModifier(speedModifier);
    }
}

[Serializable]
public struct StatusEffects
{
    public float damagePerInstance;
    public float stunPerInstance;
    public SpeedModifier speedModifier;
}