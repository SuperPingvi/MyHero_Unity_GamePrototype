using UnityEngine;

public class DazzlingLightAbility : Ability
{
    public float stunDuration = 2f;

    public GameObject aoeIndicator;
    public override GameObject GetIndicator() => aoeIndicator;

    protected override bool IsValid(AbilityContext context)
    {
        return context.point != Vector3.zero;
    }

    protected override void Apply(AbilityContext context)
    {
        Collider[] hits = Physics.OverlapSphere(context.point, aoeRadius);

        foreach (Collider hit in hits)
        {
            EnemyController_New enemy = hit.GetComponent<EnemyController_New>();
            if (enemy != null)
                enemy.TriggerStun(stunDuration);
        }
    }
    
    void Start()
    {
        Debug.Log($"aoeIndicator={aoeIndicator}, aoeRadius={aoeRadius}");
        float diameter = aoeRadius * 2f;
        aoeIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);
    }
}
