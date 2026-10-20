using System;
using UnityEngine;

public class BotHealthStats : MonoBehaviour, IUnitHealth
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject model;

    public float Health => health;
    public float MaxHealth => maxHealth;
    
    public event Action<GameObject> OnDeathEvent;
    public event Action<KillInfo> OnDeathContextEvent;
    
    private void Start()
    {
        Resurrect();
    }
    
    public void AddHealth(float value) 
        => health = Mathf.Clamp(health + value, 0, MaxHealth);

    public void TakeDamage(float damage,  GameObject attacker)
    {
        if(health <= 0) return;
        
        health -= damage;
        if (health <= 0) Die(attacker);
    }

    public void Die(GameObject attacker)
    {
        health = 0;
        OnDeathEvent?.Invoke(gameObject);
        OnDeathContextEvent?.Invoke(new KillInfo(gameObject, attacker));
        SetModelVisible(false);
    }
    
    public void Resurrect()
    {
        health = maxHealth;
        SetModelVisible(true);
        foreach (var weapon in GetComponentsInChildren<WeaponController>(true))
            weapon.ResetReload();
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
