using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public enum Faction
    {
        Friend,
        Enemy,
        Neutral
    }

    public Faction faction;
    
    [Header("Health")]
    public int currentHealth = 100;
    public int maxHealth = 100;
    public bool canBeDamaged = true;
    public bool canBeHealed;
    public bool isDead;
    
    [Header("Combat")]
    public int damage = 50;
    public float attackSpeed = 1f;
    public float attackDelay = 0.5f;
    int incomingDamage;
    
    [Header("Mana")]
    public int currentMana;
    public int maxMana = 100;
    
    [Header("Other")]
    public int shieldAmount;
    public bool hasShield => shieldAmount > 0;
    public System.Action<bool> OnShieldStateChanged;

    void Start()
    {
        currentMana = maxMana;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && other.gameObject.layer != this.gameObject.layer)
        {
            incomingDamage = other.gameObject.GetComponent<DamageCollider>().damage;
            ModifyHealth(-incomingDamage);
        }
    }
    
    public void ModifyHealth(int amount)
    {
        if (amount < 0 && !canBeDamaged) return;
        if (amount > 0 && !canBeHealed) return;

        if (amount < 0 && shieldAmount > 0)
        {
            bool hadShield = shieldAmount > 0;
            shieldAmount += amount;
            
            if (shieldAmount < 0) shieldAmount = 0;
            
            bool hasShieldNow = shieldAmount > 0;
            if (hadShield != hasShieldNow)
            {
                OnShieldStateChanged?.Invoke(hasShieldNow);
            }
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        HUDController.hud.UpdateHP();

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameObject.SetActive(false);
        }
    }

    public void ModifyMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        HUDController.hud.UpdateHP();
    }
    
    
}
