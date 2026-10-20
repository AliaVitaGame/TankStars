using System;
using UnityEngine;

public class PlayerHealthStats : MonoBehaviour, IUnitHealth
{
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private GameObject model;
    
    public float Health => health;
    public float MaxHealth => maxHealth;
    
    public event Action<GameObject> OnDeathEvent;
    public event Action<KillInfo> OnDeathContextEvent;

    public event Action PlayerDeathEvent;

    private void Start()
    {
        Resurrect();
    }

    public void AddHealth(float value) 
        => health = Mathf.Clamp(health + value, 0, MaxHealth);
    
    public void TakeDamage(float damage, GameObject attacker)
    {
        if(health <= 0) return;
        
        health -= damage;
        if (health <= 0) Die(attacker);
    }

    public void Die(GameObject attacker)
    {
        health = 0;
        PlayerDeathEvent?.Invoke();
        OnDeathEvent?.Invoke(gameObject);
        OnDeathContextEvent?.Invoke(new KillInfo(gameObject, attacker));
        SetModelVisible(false);
    }

    public void Resurrect()
    {
        health = maxHealth;
        SetModelVisible(true);
        ResetCombat();
    }

    private void ResetCombat()
    {
        foreach (var weapon in GetComponentsInChildren<WeaponController>(true))
            weapon.ResetReload();

        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void SetModelVisible(bool visible)
    {
        if (model == null)
            return;

        if (model != gameObject)
        {
            model.SetActive(visible);
            return;
        }

        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            renderer.enabled = visible;
        foreach (var controller in GetComponentsInChildren<BaseVehicleController>(true))
            controller.enabled = visible;
        foreach (var col in GetComponentsInChildren<Collider>(true))
            col.enabled = visible;
    }
}
