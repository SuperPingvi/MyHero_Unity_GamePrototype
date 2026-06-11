using UnityEngine;

public class UnitFX : MonoBehaviour
{
    public GameObject[] bloodPrefabs;
    public GameObject corpsePrefab;
    CharacterStats stats;

    void Awake()
    {
        stats = GetComponent<CharacterStats>();
        stats.OnDamaged += SpawnBlood;
        stats.OnDeath += SpawnCorpse;
    }

    void OnDestroy()
    {
        stats.OnDamaged -= SpawnBlood;
        stats.OnDeath -= SpawnCorpse;
    }

    void SpawnCorpse()
    {
        if (corpsePrefab == null) return;
        Instantiate(corpsePrefab, transform.position, transform.rotation);
    }

    void SpawnBlood()
    {
        if (bloodPrefabs.Length == 0) return;
        GameObject prefab = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];
        GameObject blood = Instantiate(prefab, transform.position, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    
        SpriteRenderer sr = blood.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(
                sr.color.r * Random.Range(0.8f, 1f),
                sr.color.g * Random.Range(0.8f, 1f),
                sr.color.b * Random.Range(0.8f, 1f)
            );
    }
}
