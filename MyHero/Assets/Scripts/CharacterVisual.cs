using System.Collections;
using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite baseSprite;

    [Header("Hero Sprites")]
    [SerializeField] Sprite advancingSprite;
    [SerializeField] Sprite chasingSprite;
    [SerializeField] Sprite attackFlashSprite;
    [SerializeField] float attackFlashDuration = 0.15f;

    HeroController heroController;
    Coroutine oneShotCoroutine;

    void Start()
    {
        heroController = GetComponent<HeroController>();
        if (heroController != null)
        {
            heroController.OnStartedAdvancing += HandleAdvancing;
            heroController.OnStartedChasing += HandleChasing;
            heroController.OnAttackSwing += HandleAttackSwing;
            if (advancingSprite != null)
                SetBaseSprite(advancingSprite);
        }
    }

    void OnDestroy()
    {
        if (heroController != null)
        {
            heroController.OnStartedAdvancing -= HandleAdvancing;
            heroController.OnStartedChasing -= HandleChasing;
            heroController.OnAttackSwing -= HandleAttackSwing;
        }
    }

    public void SetBaseSprite(Sprite sprite)
    {
        baseSprite = sprite;
        spriteRenderer.sprite = sprite;
    }

    public void PlayOneShot(Sprite sprite, float duration)
    {
        if (oneShotCoroutine != null)
            StopCoroutine(oneShotCoroutine);
        oneShotCoroutine = StartCoroutine(OneShotRoutine(sprite, duration));
    }

    IEnumerator OneShotRoutine(Sprite sprite, float duration)
    {
        spriteRenderer.sprite = sprite;
        yield return new WaitForSeconds(duration);
        spriteRenderer.sprite = baseSprite;
        oneShotCoroutine = null;
    }

    void HandleAdvancing() => SetBaseSprite(advancingSprite);
    void HandleChasing() => SetBaseSprite(chasingSprite);
    void HandleAttackSwing() => PlayOneShot(attackFlashSprite, attackFlashDuration);
}
