using System;
using UnityEngine;

public class LimbSubject : MonoBehaviour
{
    public event Action<float> OnHealthChanged;
    public event Action<float> OnHealthChangedAlpha;
    public event Action<CreatureLogic> OnCreatureDied;

    [SerializeField] float maximumHealth = 10;
    [SerializeField] float currentHealth = 10;
    public StatusManager ownStatusManager { private set; get; }
    public float MaximumHealth => maximumHealth;


    public float CurrentHealth
    {
        get => currentHealth;
        set => SetCurrentHealth(value);
    }

    void OnValidate()
    {
        SetCurrentHealth(maximumHealth);
        ownStatusManager = transform.parent.GetComponent<StatusManager>();

        if (!ownStatusManager)
            ownStatusManager = transform.parent.parent.GetComponent<StatusManager>();

    }

    public void AddHealth(float additionalHealth)
    {
        SetCurrentHealth(currentHealth + additionalHealth);
    }
    public void AddDamage(float additionalHealth)
    {
        SetCurrentHealth(currentHealth - additionalHealth);
    }

    public void SetCurrentHealth(float newHealth)
    {
        newHealth = Mathf.Clamp(newHealth, 0f, maximumHealth);

        currentHealth = newHealth;

        OnHealthChanged?.Invoke(currentHealth);
        OnHealthChangedAlpha?.Invoke(currentHealth / maximumHealth);
    }

    public void RegisterOnHealthChanged(Action<float> callback, bool getInstantCallback = false)
    {
        OnHealthChanged += callback;
        if (getInstantCallback)
            callback(currentHealth);
    }

    public void RegisterOnHealthChangedAlpha(Action<float> callback, bool getInstantCallback = false)
    {
        OnHealthChangedAlpha += callback;
        if (getInstantCallback)
            callback(currentHealth / maximumHealth);
    }
}