using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleController : MonoBehaviour
{
    [SerializeField] float detachCheckTime = 3;
    [SerializeField] TentacleTargetManager tentacleTargetManager;
    [SerializeField] PlayerDetect cursorDetect;
    [SerializeField] PlayerDetect playerDetect;
    
    bool targetCursor;
    PlayerInputActions inputActions;
    [SerializeField] List<LimbSubject> savedPossibleTarget;
    Coroutine checkIfStillInRangeCO;

    void Awake()
    {
        tentacleTargetManager = GetComponent<TentacleTargetManager>();
        inputActions = new();

        inputActions.Player.Attack.performed += ctx => Attack();
        inputActions.Player.AutoAttack.performed += ctx => AttackNear();
    }

    public void OnEnable()
    {
        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Disable();
    }

    void OnDestroy()
    {
        inputActions.Player.Attack.performed -= ctx => Attack();
        inputActions.Player.AutoAttack.performed -= ctx => AttackNear();
    }

    void Attack()
    {
        if (!cursorDetect.HasTargets)
        {
            targetCursor = !targetCursor;
            tentacleTargetManager.SetOneTarget(targetCursor ? cursorDetect.transform : null);
        }
        else
        {
            targetCursor = true;
            for (int i = 0; i < cursorDetect.PossibleTargets.Count; i++)
            {
                if (!savedPossibleTarget.Contains(cursorDetect.PossibleTargets[i]))
                    savedPossibleTarget.Add(cursorDetect.PossibleTargets[i]);
            }
            tentacleTargetManager.SetAttackStatusManager(cursorDetect.PossibleTargets);

            if (checkIfStillInRangeCO != null)
                StopCoroutine(checkIfStillInRangeCO);
            checkIfStillInRangeCO = StartCoroutine(CheckIfStillInRange());
        }

    }

    void AttackNear()
    {
        if (playerDetect == null)
        {
            Debug.Log("Doesnt have: " + nameof(playerDetect) + " in " + name, this);
            return;
        }
        
        if (!playerDetect.HasTargets)
        {
            targetCursor = !targetCursor;
            tentacleTargetManager.SetOneTarget(targetCursor ? playerDetect.transform : null);
        }
        else
        {
            targetCursor = true;
            for (int i = 0; i < playerDetect.PossibleTargets.Count; i++)
            {
                if (!savedPossibleTarget.Contains(playerDetect.PossibleTargets[i]))
                    savedPossibleTarget.Add(playerDetect.PossibleTargets[i]);
            }
            tentacleTargetManager.SetAttackStatusManager(playerDetect.PossibleTargets);

            if (checkIfStillInRangeCO != null)
                StopCoroutine(checkIfStillInRangeCO);
            checkIfStillInRangeCO = StartCoroutine(CheckIfStillInRange());
        }

    }

    IEnumerator CheckIfStillInRange()
    {
        yield return new WaitForSeconds(detachCheckTime);

        for (int i = 0; i < savedPossibleTarget.Count; i++)
        {
            if (!playerDetect.PossibleTargets.Contains(savedPossibleTarget[i]))
            {
                savedPossibleTarget.RemoveAt(i);
                i--;
            }
            tentacleTargetManager.SetAttackStatusManager(savedPossibleTarget);
        }

        if (savedPossibleTarget.Count == 0)
        {
            targetCursor = false;
            tentacleTargetManager.ResetTentacles();
        }
        else
            checkIfStillInRangeCO = StartCoroutine(CheckIfStillInRange());
    }
    
}