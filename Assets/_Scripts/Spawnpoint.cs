using UnityEngine;
using System.Collections;

public class Spawnpoint : MonoBehaviour
{
    public Transform spawnPoint;
    public AnimationClip deathClip;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;
    bool dead;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Kill(Vector2 knockbackDir, float force, float delay)
    {
        if (dead) return;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        dead = true;

        float actualDelay = delay;
        if (deathClip != null)
        {
            actualDelay = deathClip.length;
        }

        StartCoroutine(DeathRoutine(knockbackDir, force, delay));
    }

    IEnumerator DeathRoutine(Vector2 knockbackDir, float force, float delay)
    {
        rb.velocity = Vector2.zero;

        rb.AddForce(knockbackDir.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(delay);

        Respawn();
    }

    void Respawn()
    {
        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        rb.velocity = Vector2.zero;
        transform.position = spawnPoint.position;
        dead = false;

        MapLayerManager manager = FindObjectOfType<MapLayerManager>();
        if (manager != null)
            manager.ResetToOriginalLayer();
    }
}
