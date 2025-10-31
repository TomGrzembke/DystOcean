using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    [SerializeField] Transform middle;
    public List<LimbSubject> PossibleTargets => possibleTargets;
    [SerializeField] List<LimbSubject> possibleTargets = new();
    public bool HasTargets => possibleTargets.Count > 0;
    public LayerMask CreatureLayer => creatureLayer;
    [SerializeField] LayerMask creatureLayer;
    [SerializeField] float detectionRadius;
    [SerializeField] StatusManager playerStatusManager;

    void Update()
    {
        DetectTargets();
    }

    void DetectTargets()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(middle.position, detectionRadius, creatureLayer);
        possibleTargets.Clear();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].TryGetComponent(out LimbSubject limbTarget))
                limbTarget = colliders[i].GetComponentInChildren<LimbSubject>();

            if (!limbTarget) continue;
            if (limbTarget.OwnStatusManager == playerStatusManager) continue;

            if (!possibleTargets.Contains(limbTarget))
                possibleTargets.Add(limbTarget);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, detectionRadius); //This visualizes the detection radius
    }
#endif
}