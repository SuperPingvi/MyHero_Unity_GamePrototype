using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityRegistry : MonoBehaviour
{
    public static AbilityRegistry instance;

    private PlayerController playerController;
    
    private static HashSet<string> unlockedAbilities = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerController = PartyManager.instance.player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("AbilityRegistry: No PlayerController found in scene!");
            return;
        }
        ApplySavedState();
    }

    public void Unlock(string abilityName)
    {
        Ability ability = GetAbility(abilityName);
        if (ability == null) return;

        unlockedAbilities.Add(abilityName);
        ability.enabled = true;
        playerController.RefreshAbilities();
    }

    public void Lock(string abilityName)
    {
        Ability ability = GetAbility(abilityName);
        if (ability == null) return;

        unlockedAbilities.Remove(abilityName);
        ability.enabled = false;
        playerController.RefreshAbilities();
    }

    public bool IsUnlocked(string abilityName)
    {
        return unlockedAbilities.Contains(abilityName);
    }

    private void ApplySavedState()
    {
        var abilities = playerController.GetComponents<Ability>();
        foreach (var ability in abilities)
        {
            ability.enabled = IsUnlocked(ability.abilityName);
        }
        playerController.RefreshAbilities();
    }

    private Ability GetAbility(string abilityName)
    {
        var abilities = playerController.GetComponents<Ability>();
        foreach (var ability in abilities)
        {
            if (ability.abilityName == abilityName)
                return ability;
        }
        Debug.LogWarning($"Ability not found: {abilityName}");
        return null;
    }

    public void ReserAll()
    {
        unlockedAbilities.Clear();
        ApplySavedState();
    }
}
