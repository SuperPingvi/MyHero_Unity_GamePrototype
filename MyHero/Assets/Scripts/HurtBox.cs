using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    CharacterStats ownerStats;
    readonly HashSet<Collider> activeHitColliders = new HashSet<Collider>();

    void Awake()
    {
        ownerStats = GetComponentInParent<CharacterStats>();
        if (!ownerStats)
            Debug.LogError($"HurtBox on '{gameObject.name}' found no CharacterStats in parent hierarchy.", this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Weapon")) return;

        var dmgCollider = other.GetComponent<DamageCollider>();
        if (dmgCollider == null) return;

        var attackerStats = other.GetComponentInParent<CharacterStats>();
        if (attackerStats != null && attackerStats.faction == ownerStats.faction) return;

        if (!activeHitColliders.Add(other)) return;

        ownerStats.ModifyHealth(-dmgCollider.damage);
    }

    void OnDisable()
    {
        activeHitColliders.Clear();
    }

    void OnTriggerExit(Collider other)
    {
        activeHitColliders.Remove(other);
    }
}
