using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public float cooldown = 3f;
    public float range = 3f;
    
    protected float lastUseTime;

    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void TryUse(GameObject target)
    {
        if (!CanUse()) return;
        
        Use(target);
        lastUseTime = Time.time;
    }
    
    protected abstract void Use(GameObject target);
    
    protected virtual bool IsValidTarget(GameObject target)
    {
        return true;
    }
}
