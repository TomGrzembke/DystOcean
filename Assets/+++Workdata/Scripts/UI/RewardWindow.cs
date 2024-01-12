using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardWindow : MonoBehaviour
{
    public static RewardWindow Instance;

    #region serialized fields
    [SerializeField] GameObject rewardWindow;
    [SerializeField] GameObject essentialUI;
    [SerializeField] Image rewardImage;
    [SerializeField] TextMeshProUGUI titelText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] float fadeTime = 2;
    [SerializeField] TextMeshProUGUI rewardText;
    #endregion

    #region private fields
    CanvasGroup rewardWindowCanvasGroup;
    CanvasGroup essentialUICanvasGroup;

    #endregion

    void Awake()
    {
        Instance = this;
        rewardWindowCanvasGroup = rewardWindow.GetComponent<CanvasGroup>();
        essentialUICanvasGroup = essentialUI.GetComponent<CanvasGroup>();
    }

    [ButtonMethod]
    public void GiveReward()
    {
        StopAllCoroutines();
        StartCoroutine(ShowCoroutine());
    }

    public void GiveReward(GameObject reward)
    {
        if (!reward) return;
        Ability ability = reward.GetComponent<Ability>();
        rewardImage.sprite = ability.AbilitySO.abilitySprite;
        titelText.text = ability.AbilitySO.abilityTitel;
        descriptionText.text = ability.AbilitySO.abilityDescription;

        PauseManager.Instance.PauseLogic(true);
        StopAllCoroutines();
        StartCoroutine(ShowCoroutine());
    }

    [ButtonMethod]
    public void Close()
    {
        PauseManager.Instance.PauseLogic(false);
        StopAllCoroutines();
        StartCoroutine(HideCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        essentialUI.SetActive(true);
        rewardWindow.SetActive(true);
        essentialUICanvasGroup.alpha = 1;
        rewardWindowCanvasGroup.alpha = 0;
        float time = 0;

        while (time < fadeTime)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            essentialUICanvasGroup.alpha = 1 - Mathf.Clamp01(time / fadeTime);
            rewardWindowCanvasGroup.alpha = Mathf.Clamp01(time / fadeTime);
        }
        rewardWindowCanvasGroup.alpha = 1;
        essentialUICanvasGroup.alpha = 0;
    }

    IEnumerator HideCoroutine()
    {
        essentialUICanvasGroup.alpha = 0;
        essentialUI.SetActive(true);
        rewardWindowCanvasGroup.alpha = 0;
        rewardWindow.SetActive(false);
        float time = 0;

        while (time < fadeTime)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            essentialUICanvasGroup.alpha = Mathf.Clamp01(time / fadeTime);
        }
        essentialUICanvasGroup.alpha = 1;
    }
}