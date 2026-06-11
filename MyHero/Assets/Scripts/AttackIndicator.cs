using UnityEngine;
using UnityEngine.UI;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private float duration;
    private float timer;
    private bool isActive;

    public void Show(float windupDuration)
    {
        duration = windupDuration;
        timer = 0f;
        isActive = true;

        gameObject.SetActive(true);
        fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;

        fillImage.fillAmount = timer / duration;

        if (timer >= duration)
        {
            isActive = false;
        }
    }

    public void Hide()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
