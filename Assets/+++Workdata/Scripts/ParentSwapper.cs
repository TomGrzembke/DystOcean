using MyBox;
using System.Collections;
using UnityEngine;

public class ParentSwapper : MonoBehaviour
{
    #region serialized fields
    [SerializeField] bool disablePlayer;
    [SerializeField, ConditionalField(nameof(disablePlayer))] PlayerMovement playerMovement;
    [SerializeField] Transform targetTrans;
    [SerializeField] float timeTill0Rot = 4;
    #endregion

    #region private fields
    Transform originalParent;
    Transform obj;
    #endregion

    public void Unparent()
    {
        targetTrans.parent = null;
        StartCoroutine(RotateTill0());
    }

    public void Swap(Transform _obj, Transform tempParent)
    {
        obj = _obj;
        originalParent = obj.parent;
        obj.parent = tempParent;

        if (disablePlayer)
            playerMovement.SetControlState(PlayerMovement.ControlState.GAME_CONTROL);
    }

    public void UnSwap()
    {
        obj.parent = originalParent;

        if (disablePlayer)
            playerMovement.ReenableMovement();
    }

    IEnumerator RotateTill0()
    {
        float rotateTime = 0;
        Quaternion startRotation = targetTrans.rotation;
        while (rotateTime < timeTill0Rot)
        {
            rotateTime += Time.deltaTime;
            targetTrans.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, rotateTime / timeTill0Rot);
            yield return null;
        }
    }
}