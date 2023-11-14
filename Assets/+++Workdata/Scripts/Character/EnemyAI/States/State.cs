using UnityEngine;

public abstract class State : MonoBehaviour
{
    #region private fields

    protected CreatureLogic creatureLogic;

    #endregion

    void Awake() => creatureLogic = GetComponentInParent<CreatureLogic>();

    public abstract State SwitchState();

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