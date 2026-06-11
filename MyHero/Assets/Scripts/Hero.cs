using UnityEngine;

public class Hero : Interactable
{
    public CharacterStats selfStats;
    public HeroController heroController;
    
    [SerializeField] private Sprite aimSprite;
    private EffectVisual effectVisual;
    
    void Start()
    {
        if (heroController == null) heroController = GetComponent<HeroController>();
        if (selfStats == null) selfStats = GetComponent<CharacterStats>();
        if (effectVisual == null) effectVisual = GetComponent<EffectVisual>();
    }

    public override void Interact()
    {
        base.Interact();
        var ac = player?.GetComponent<AbilityController>();
        Debug.Log($"Hero.Interact called. player={player}, ac={ac}, hasAbility={ac?.HasAbility()}");

        if (ac != null && ac.HasAbility())
        {
            Debug.Log($"Firing ability on {selfStats}");
            var targetStats = GetComponent<CharacterStats>();
            if (ac.UseOnTarget(targetStats))
                player?.GetComponent<PlayerController>()?.OnAbilityCastSuccess();
        }
    }
    
    public void ShowAimIndicator() => effectVisual?.ShowEffect(aimSprite);
    
    public void ClearAimIndicator() => effectVisual?.ClearEffect();
}
