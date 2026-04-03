using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
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

    void Start()
    {
        currentMana = maxMana;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && other.gameObject.layer != this.gameObject.layer)
        {
            incomingDamage = other.gameObject.GetComponentInParent<CharacterStats>().damage;
            ReceiveDamage();
        }
    }
        void ReceiveDamage()
    {
        if (!isDead)
        {
            currentHealth -= incomingDamage;
            HUDController.hud.UpdateHP();
            if (currentHealth <= 0)
            {
                isDead = true;
                gameObject.SetActive(false);
            }
        }
    }
    
    public void ModifyHealth(int amount)
    {
        if (amount < 0 && !canBeDamaged)
            return;

        if (amount > 0 && !canBeHealed)
            return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        HUDController.hud.UpdateHP();
    }
}
