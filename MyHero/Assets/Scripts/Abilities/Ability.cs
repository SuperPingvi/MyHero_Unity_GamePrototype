using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Base")]
    public Sprite abilityIcon;
    public Sprite iconSelected;
    public Sprite iconInactive;
    public string abilityName;
    public int manaCost;
    public float cooldown = 3f;
    public float range = 3f;
    
    [Header("Targeting")]
    public TargetType targetType;
    public AbilityCastType castType;
    
    protected float lastUseTime;
    
    public enum TargetType
    {
        Ally,
        Enemy,
        Self,
        Any,
        Ground
    }
    
    public enum AbilityCastType
    {
        Targeted,
        Instant,
        AOE
    }    

    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, (lastUseTime + cooldown) - Time.time);
    }

    public void StartCooldown()
    {
        lastUseTime = Time.time;
    }
    
    public bool TryUse(AbilityContext context)
    {
        Debug.Log($"TryUse: CanUse={CanUse()}, IsValid={IsValid(context)}");
        if (!CanUse()) return false;

        if (context.caster.currentMana < manaCost)
            return false;
        
        if (!IsValid(context)) return false;
        
        Apply(context);
        context.caster.ModifyMana(-manaCost);
        StartCooldown();
        return true;
    }

    protected virtual bool IsValid(AbilityContext context)
    {
        // Ground target
        if (targetType == TargetType.Ground)
            return context.point != Vector3.zero;
        
        // Target required
        if (context.target == null)
            return false;

        switch (targetType)
        {
            case TargetType.Self:
                return context.target == context.caster;
            
            case TargetType.Any:
                return true;
            
            case TargetType.Ally:
                return context.target.faction == context.caster.faction;
            
            case TargetType.Enemy:
                return context.target.faction != context.caster.faction;
            
        }
        return false;
    }
    
    protected abstract void Apply(AbilityContext context);

}
