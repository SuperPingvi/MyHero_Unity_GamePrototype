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
    
    [Header("Mana")]
    public int currentMana;
    public int maxMana = 100;
    
    [Header("Other")]
    public int shieldAmount;
    public bool hasShield => shieldAmount > 0;
    public System.Action<bool> OnShieldStateChanged;
    public System.Action OnDamaged;
    public System.Action OnDeath;

    void Start()
    {
        currentMana = maxMana;
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
        if (amount < 0)
            OnDamaged?.Invoke();
        HUDController.hud.UpdateHP();

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
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
