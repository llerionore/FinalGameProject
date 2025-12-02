using UnityEngine;
using System.Collections;

public class Spawnpoint : MonoBehaviour
{
    public Transform spawnPoint;
    private Rigidbody2D rb;
    bool dead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Kill(Vector2 knockbackDir, float force, float delay)
    {
        if (dead) return;

        dead = true;
        StartCoroutine(DeathRoutine(knockbackDir, force, delay));
    }

    IEnumerator DeathRoutine(Vector2 knockbackDir, float force, float delay)
    {
        // сбрасываем скорость и дэш
        rb.velocity = Vector2.zero;

        // нокбек
        rb.AddForce(knockbackDir.normalized * force, ForceMode2D.Impulse);

        // ждём
        yield return new WaitForSecondsRealtime(delay);

        Respawn();
    }

    void Respawn()
    {
        rb.velocity = Vector2.zero;
        transform.position = spawnPoint.position;
        dead = false;
    }
}
