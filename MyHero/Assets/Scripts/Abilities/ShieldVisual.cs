using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private GameObject shieldObject;

    private CharacterStats stats;

    private void Start()
    {
        stats = GetComponent<CharacterStats>();

        if (stats != null)
        {
            stats.OnShieldStateChanged += ToggleShield;
            ToggleShield(stats.hasShield);
        }
    }

    private void OnDestroy()
    {
        if (stats != null)
            stats.OnShieldStateChanged -= ToggleShield;
    }

    private void ToggleShield(bool active)
    {
        shieldObject.SetActive(active);
    }
}
