using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class GrabTransManager : MonoBehaviour
{
    [SerializeField] List<Transform> grabTrans = new();

    [ButtonMethod]
    public void GatherChildren()
    {
        grabTrans.Clear();
        for (int i = 0; i < transform.childCount; i++)
            grabTrans.Add(transform.GetChild(i));
    }

    public Transform GetClosestGrabTrans(Vector3 pos)
    {
        float closestDistance = 100;
        Transform closestTrans = null;

        for (int i = 0; i < grabTrans.Count; i++)
        {
            float distance = Vector3.Distance(grabTrans[i].position, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTrans = grabTrans[i];
            }
        }

        return closestTrans;
    }

    public Transform GetACloseGrabTrans(Vector3 pos)
    {
        float closestDistance = 100;
        Transform closestTrans = grabTrans[0];

        for (int i = 0; i < grabTrans.Count; i++)
        {
            float distance = Vector3.Distance(grabTrans[i].position, pos);
            if (distance < closestDistance)
            {
                if (70 > Random.Range(0, 101))
                {
                    closestDistance = distance;
                    closestTrans = grabTrans[i];
                }
            }
        }

        return closestTrans;
    }

    public Transform GetRandomGrabTrans()
    {
        int randomNumber = Random.Range(0, grabTrans.Count);

        return grabTrans[randomNumber];
    }
}