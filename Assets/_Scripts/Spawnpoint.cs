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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Kill(Vector2 knockbackDir, float force, float delay)
    {

        if (dead) return;
        dead = true;

        playerMovement.isDead = true;
        rb.velocity = Vector2.zero;

        animator.SetBool("isDead", true);

        ScreenFlash.instance.Flash();

        float deathDuration = deathClip != null ? deathClip.length : delay;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        StartCoroutine(DeathRoutine(knockbackDir, force, deathDuration));
    }

    IEnumerator DeathRoutine(Vector2 knockbackDir, float force, float delay)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(delay);

        Respawn();
    }

    void Respawn()
    {
        animator.SetBool("isDead", false);

        playerMovement.isDead = false;

        rb.velocity = Vector2.zero;
        transform.position = spawnPoint.position;

        dead = false;

        MapLayerManager manager = FindObjectOfType<MapLayerManager>();
        if (manager != null)
            manager.ResetToOriginalLayer();
    }
}
