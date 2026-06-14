using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Interactable
{
    public CharacterStats selfStats;
    public EnemyController_New enemyController;
    void Start()
    {
        if (selfStats == null)
            selfStats = gameObject.GetComponent<CharacterStats>();
        if (enemyController == null)
            enemyController = gameObject.GetComponent<EnemyController_New>();
    }
    public override void Interact()
    {
        base.Interact();
        Attack();
    }
    void Attack()
    {
        Debug.Log("Attacking" + transform.name);
        enemyController.TriggerStun(enemyController.stunDuration);
    }
}
