using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public float knockbackForce = 10f;
    public float deathDelay = 0.3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();

            if (pm != null && pm.isInvincible)
                return;

            Spawnpoint respawn = collision.GetComponent<Spawnpoint>();

            if (respawn != null)
            {
                Vector2 dir = Vector2.up;
                respawn.Kill(dir, knockbackForce, deathDelay);
                Time.timeScale = 1;
            }
        }
    }
}
