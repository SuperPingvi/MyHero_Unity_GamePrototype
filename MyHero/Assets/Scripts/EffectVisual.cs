using UnityEngine;
using UnityEngine.UI;

public class EffectVisual : MonoBehaviour
{
    [SerializeField] private Image effectImage;

    public void ShowEffect(Sprite effect)
    {
        effectImage.sprite = effect;
        effectImage.enabled = true;
    }

    public void ClearEffect()
    {
        effectImage.enabled = false;
        effectImage.sprite = null;
    }
}