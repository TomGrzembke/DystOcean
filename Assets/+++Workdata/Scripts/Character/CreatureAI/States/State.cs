using UnityEngine;

public abstract class State : MonoBehaviour
{
    #region private fields

    protected CreatureLogic creatureLogic;

    #endregion

    #region
    [SerializeField] protected State uniqueState;
    #endregion

    void Awake() => creatureLogic = GetComponentInParent<CreatureLogic>();

    public State SwitchState()
    {
        if (!uniqueState)
            return SwitchStateInternal();

        if (!uniqueState.uniqueState)
            return uniqueState.SwitchStateInternal();

        for (int i = 0; i < Mathf.Infinity; i++)
        {
            if (!uniqueState.uniqueState)
                break;

            uniqueState = uniqueState.uniqueState;
        }

        return uniqueState.SwitchStateInternal();
    }

    public abstract State SwitchStateInternal();

    /// <summary>
    /// The time since the state was entered.
    /// </summary>
    public float TimeInState;

    /// <summary>
    /// The fixed time since the state was entered, for any physics tests.
    /// </summary>
    public float FixedTimeInState;

    public void EnterState()
    {
        TimeInState = 0;
        FixedTimeInState = 0;
        EnterInternal();
    }


    protected abstract void EnterInternal();

    public void UpdateState()
    {
        TimeInState += Time.deltaTime;
        UpdateInternal();
    }

    protected abstract void UpdateInternal();

    public void FixedUpdateState()
    {
        FixedTimeInState += Time.fixedDeltaTime;
        FixedUpdateInternal();
    }

    protected abstract void FixedUpdateInternal();

    public void ExitState()
    {
        TimeInState = 0;
        FixedTimeInState = 0;
        ExitInternal();
    }

    protected abstract void ExitInternal();
}