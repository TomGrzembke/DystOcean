using System.Collections;
using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    #region serialized fields
    [SerializeField] LimbSubject limbSubject;
    [SerializeField, Range(0, 100)] float percentageEffectiveness = 100;
    [SerializeField] float regenPerTick = .1f;
    [SerializeField] float interval = .5f;
    #endregion

    #region private fields
    Coroutine regenRoutine;
    #endregion

    void OnEnable()
    {
        regenRoutine = StartCoroutine(LifeRegen());
    }

    void OnDisable()
    {
        StopCoroutine(regenRoutine);
    }

    IEnumerator LifeRegen()
    {

        yield return new WaitForSeconds(interval);
    }
}