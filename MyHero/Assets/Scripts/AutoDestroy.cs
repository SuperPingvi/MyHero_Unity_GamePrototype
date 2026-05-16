using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 0.2f;

    void OnEnable()
    {
        Destroy(gameObject, delay);
    }
}
