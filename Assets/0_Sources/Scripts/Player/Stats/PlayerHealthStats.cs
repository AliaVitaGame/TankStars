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
        model.SetActive(false);
        PlayerDeathEvent?.Invoke();
        OnDeathEvent?.Invoke(gameObject);
        OnDeathContextEvent?.Invoke(new KillInfo(gameObject, attacker));
    }

    public void Resurrect()
    {
        health = maxHealth;
        model.SetActive(true);
    }
}
