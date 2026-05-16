using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 targetPoint;
    float speed;
    bool launched;
    Vector3 startPoint;

    public void Launch(Vector3 spawnPoint, Vector3 target, float projectileSpeed)
    {
        startPoint = spawnPoint;
        targetPoint = target;
        speed = projectileSpeed;
        transform.position = spawnPoint;
        launched = true;

        Vector3 dir = (targetPoint - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void Update()
    {
        if (!launched) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            launched = false;
            transform.localPosition = Vector3.zero; // reset back to spawn
            gameObject.SetActive(false);
        }
    }
}
