using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private GameObject shieldObject;

    private void Start()
    {
        var stats = GetComponent<CharacterStats>();

        if (stats != null)
        {
            stats.OnShieldStateChanged += ToggleShield;
            ToggleShield(stats.hasShield);
        }
    }

    private void ToggleShield(bool active)
    {
        shieldObject.SetActive(active);
    }
}
