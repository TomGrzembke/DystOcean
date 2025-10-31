using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTargetManager : MonoBehaviour
{
    [SerializeField] List<TentacleBehavior> tentacles;
    [SerializeField] List<LimbSubject> targetLimbs;
    [SerializeField] List<Transform> targetTrans;
    [SerializeField] Material tentacleMaterial;
    [SerializeField] Color attackColor;
    public List<StatusManager> TargetStatusManagers => targetStatusManagers;
    [SerializeField] List<StatusManager> targetStatusManagers;
    public event Action<List<StatusManager>> OnTargetStatusManagersChanged;


    List<StatusManager> oldTargetStatusManagers;


    public void SetTargets()
    {
        RemoveAllNullTargets();
        targetTrans.Clear();

        int tentacleCount = tentacles.Count;
        int targetCount = targetLimbs.Count;

        switch (targetCount)
        {
            case 0:
                targetTrans.Clear();
                ResetTentacles();
                break;
            
            case 1:
                if (tentacleMaterial)
                    tentacleMaterial.color = attackColor;
                for (int i = 0; i < tentacleCount; i++)
                {
                    AddTargetTrans(targetLimbs[0].OwnStatusManager.GrabManager.GetRandomGrabTrans());
                }
                break;
            
            default:
                if (tentacleMaterial)
                    tentacleMaterial.color = attackColor;
                for (int i = 0; i < tentacleCount; i++)
                {
                    AddTargetTrans(targetLimbs[UnityEngine.Random.Range(0, targetCount)].OwnStatusManager.GrabManager.GetACloseGrabTrans(transform.position));
                }
                break;
        }

        targetTrans.Shuffle();

        for (int i = 0; i < tentacleCount; i++)
        {
            if (i > targetTrans.Count - 1)
                break;
            tentacles[i].SetGrabTarget(targetTrans[i].transform);
        }
    }

    public void ResetTentacles(Transform newTarget = null)
    {
        if (tentacleMaterial)
            tentacleMaterial.color = Color.white;
        for (int i = 0; i < tentacles.Count; i++)
        {
            tentacles[i].SetGrabTarget(newTarget);
        }
    }

    void AddTargetTrans(Transform singleTarget)
    {
        targetTrans.Add(singleTarget);
    }

    void RemoveAllNullTargets()
    {
        for (int i = 0; i < targetLimbs.Count; i++)
        {
            if (targetLimbs[i] == null)
            {
                targetLimbs.RemoveAt(i);
                i--;
            }
        }
    }

    public void AddAttackStatusManager(LimbSubject attackTarget)
    {
        targetLimbs.Clear();
        targetLimbs.Add(attackTarget);
        RecalculateTargetStatusManager();
        SetTargets();
    }

    public void SetAttackStatusManager(List<LimbSubject> attackTarget)
    {
        targetLimbs.Clear();

        for (int i = 0; i < attackTarget.Count; i++)
        {
            if (!targetLimbs.Contains(attackTarget[i]))
            {
                targetLimbs.Add(attackTarget[i]);
            }
        }

        RecalculateTargetStatusManager();

        SetTargets();
    }

    void RecalculateTargetStatusManager()
    {
        oldTargetStatusManagers = new List<StatusManager>(targetStatusManagers);

        targetStatusManagers.Clear();
        for (int i = 0; i < targetLimbs.Count; i++)
        {
            StatusManager targetStatusManager = targetLimbs[i].OwnStatusManager;
            if (!targetStatusManagers.Contains(targetStatusManager))
            {
                targetStatusManagers.Add(targetStatusManager);
            }
        }

        if (oldTargetStatusManagers != targetStatusManagers)
            OnTargetStatusManagersChanged?.Invoke(targetStatusManagers);
    }

    public void RemoveAttackStatusManager(LimbSubject attackTarget)
    {
        if (targetLimbs.Remove(attackTarget))
            SetTargets();
    }

    public void SetOneTarget(Transform target)
    {
        targetLimbs.Clear();
        targetTrans.Clear();
        ResetTentacles(target);
    }

    public void AddTentacle(GameObject newTentacle)
    {

    }

    public void SetTentacles(bool condition)
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            tentacles[i].TryGetComponent(out TentacleDetection _tentacleDetection);
            if (_tentacleDetection)
                _tentacleDetection.enabled = condition;
        }
    }

    public void RegisterOnTargetStatusManagersChanged(Action<List<StatusManager>> callback, bool getInstantCallback = false)
    {
        OnTargetStatusManagersChanged += callback;
        if (getInstantCallback)
            callback(targetStatusManagers);
    }

    void OnApplicationQuit()
    {
        if (tentacleMaterial)
            tentacleMaterial.color = Color.white;
    }
}