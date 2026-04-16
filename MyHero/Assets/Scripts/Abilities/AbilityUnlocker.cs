using UnityEngine;

public class LevelAbilityUnlocker : MonoBehaviour
{
    [SerializeField] private string[] abilitiesToUnlock;

    private void Start()
    {
        foreach (var abilityName in abilitiesToUnlock)
        {
            AbilityRegistry.instance.Unlock(abilityName);
        }
    }
}
