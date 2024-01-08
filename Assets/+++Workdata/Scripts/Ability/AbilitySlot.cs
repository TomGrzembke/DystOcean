using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    #region serialized fields
    public GameObject CurrentAbilityPrefab => currentAbilityPrefab;
    [SerializeField] GameObject currentAbilityPrefab;
    [SerializeField] int slotIndex;
    [SerializeField] Image abilityImage;
    #endregion

    #region private fields
    Ability currentAbility;
    public Ability CurrentAbility => currentAbility;
    AbilitySlotManager abilitySlotManager;
    #endregion

    void OnValidate()
    {
        if (Application.isPlaying)
            if (currentAbilityPrefab && abilitySlotManager)
                abilitySlotManager.AddNewAbility(currentAbilityPrefab, slotIndex);
        RefreshPicture();
    }
    void RefreshPicture()
    {
        if (currentAbilityPrefab)
            currentAbilityPrefab.TryGetComponent(out currentAbility);

        abilityImage.sprite = currentAbilityPrefab ? currentAbility.AbilitySprite : null;
    }

    public void ChangeAbilityPrefab(GameObject newAbilityPrefab, AbilitySlotManager _abilitySlotManager)
    {
        if (currentAbility && newAbilityPrefab != currentAbilityPrefab)
            DestroyImmediate(currentAbility.gameObject, true);
        currentAbilityPrefab = newAbilityPrefab;

        if (currentAbilityPrefab)
        {
            currentAbility = Instantiate(newAbilityPrefab, gameObject.transform).GetComponent<Ability>();
            abilitySlotManager = _abilitySlotManager;
            EnterAbility();
        }
        RefreshPicture();
    }
    public void EnterAbility()
    {
        if (currentAbility)
            currentAbility.EnterAbility(abilitySlotManager);
    }
    public void Execute(bool deactivate = false)
    {
        if (currentAbility)
            currentAbility.Execute(abilitySlotManager, deactivate);
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }
}