using System.Collections;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public float delay;
    public float spawnInterval;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
        }
        if (delay > 0)
            StartCoroutine(ActivateWithDelay());
    }

    IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            
            if (spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
        }
    }
}
