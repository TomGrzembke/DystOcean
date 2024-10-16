using UnityEngine;

public class OnPlayerDead : MonoBehaviour
{
    #region serialized fields
    [SerializeField] HealthSubject healthSubject;
    [SerializeField] TentacleTargetManager tentacleTargetManager;
    [SerializeField] Transform headTrans;
    [SerializeField] GameObject jellyFishGO;
    [SerializeField] Transform gfxTrans;
    [SerializeField] TentacleController tentacleController;
    #endregion

    #region private fields

    #endregion

    void OnEnable()
    {
        healthSubject.RegisterOnCreatureDied(OnPlayerDied);
    }
    void OnDisable()
    {
        healthSubject.OnCreatureDied -= OnPlayerDied;
    }

    void OnPlayerDied(bool dead)
    {
        if (dead)
        {
            SkillManager.OpenSkillManager();
            tentacleTargetManager.SetTentacles(false);
            gfxTrans.parent = headTrans;
            jellyFishGO.SetActive(false);
            tentacleController.enabled = false;
        }
    }
}