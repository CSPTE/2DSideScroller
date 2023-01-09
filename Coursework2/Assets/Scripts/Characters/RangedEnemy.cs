using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{

    public float viewRange = 100f;
    public Transform castPoint;
    public Transform firePoint;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public Animator animator;
    public GameObject healthBarPrefab;
    public GameObject bulletPrefab;

    public int maxHealth = 100;
    public int damage = 25;
    public float bulletVelocity;

    private int health;
    private bool facingRight = true;
    private Transform target;
    private float direction = 1f;
    private bool attacking = false;
    private bool dead = false;

    public AudioSource growlAudio;
    private bool keepPlayingGrowl = true;
    public AudioSource attackAudio;

    protected HealthBar healthBar;

    private void Start() {
        Vector3 theScale = transform.localScale;
        if (theScale.x < 0) {
            facingRight = false;
        }
        target = Player.current.transform;
        health = maxHealth;
        GameObject worldCanvas = GameObject.Find("WorldCanvas");
        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
        healthBar.SetMaxHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead) {
            if (health == 0) {
                dead = true;
            }
            // Turn to face same direction
            if (!attacking && target.position.x < transform.position.x) {
                FaceLeft();
                direction = -1f;
            } else if (!attacking) {
                FaceRight();
                direction = 1f;
            }

            if (CanSeeTarget()) {
                attacking = true;
                animator.SetBool("Attacking", true);
            }
            healthBar.SetPosition(transform.position);
        }
    }

    public void Damage(int amount) {
        health -= amount;
        if (health < 0) {
            health = 0;
        }
        if (health == 0) {
            OnDeath();
        }
        healthBar.SetHealth(health);
    }

    private bool CanSeeTarget() {
        Vector2 endPos = castPoint.position + Vector3.right * viewRange * direction;
        RaycastHit2D ray = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Action"));
        if (ray.collider != null) {
            if (ray.collider.gameObject.CompareTag("Player")) {
                return true;
            }
        }
        return false;
    }

    void Fire() {
        GameObject gameObject = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * bulletVelocity, 0f);
        gameObject.GetComponent<Bullet>().SetDamage(damage);
        animator.SetBool("Attacking", false);
        attacking = false;
        attackAudio.Play();
    }

    private void OnDeath() {
        animator.SetBool("Dead", true);
        gameObject.layer = LayerMask.NameToLayer("NPCLayer");
    }

    public void OnDeathComplete() {
        healthBar.Destroy();
        StoryEngine.current.TriggerEvent("EnemyKilled");
        Destroy(gameObject);
    }

    public void Flip() {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void FaceLeft() {
        if (facingRight) {
            Flip();
        }
    }

    public void FaceRight() {
        if (!facingRight) {
            Flip();
        }
    }
}
