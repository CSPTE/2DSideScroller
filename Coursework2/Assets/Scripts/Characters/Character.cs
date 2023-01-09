using UnityEngine;

public class Character : MonoBehaviour
{

    public float targetDistance = 3f;
    public float viewRange = 100f;
    public float chaseSpeed = 50f;
    public float idleSpeed = 10f;
    public CharacterController2D controller;
    public Transform groundAheadCheck;
    public Transform castPoint;
    public Transform wallAheadCheck;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public GameObject healthBarPrefab;
    public int maxHealth = 100;
    
    protected int health;
    protected const float groundRadius = .2f;
    protected Transform target;
    protected float direction = 1f;
    protected bool chasing = false;
    protected bool reached = false;
    protected bool dead = false;

    protected HealthBar healthBar;


    // Start is called before the first frame update
    void Start()
    {
        target = Player.current.transform;
        health = maxHealth;
        GameObject worldCanvas = GameObject.Find("WorldCanvas");
        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
        healthBar.SetMaxHealth(health);
    }

    protected virtual void OnDeath() {
    
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
        // Turn to face same direction
        if (target.position.x < transform.position.x) {
            controller.FaceLeft();
            direction = -1f;
        } else {
            controller.FaceRight();
            direction = 1f;
        }
    }

    private void Update() {
        UpdateLogic();
    }

    protected void UpdateLogic() {
        if (!dead) {
            if (health == 0) {
                dead = true;
            }
            if (CanSeeTarget()) {
                ChaseTarget();
                chasing = true;
            } else {
                chasing = false;
                StopChasingTarget();
            }
            healthBar.SetPosition(transform.position);
        }
    }

    private bool CanSeeTarget() {
        reached = false;
        Vector2 endPos = castPoint.position + Vector3.right * viewRange * direction;
        RaycastHit2D ray = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Action"));
        if (ray.collider != null) {
            if (ray.collider.gameObject.CompareTag("Player")) {
                return true;
            }
        }
        return false;
    }

    protected virtual void OnTargetReached() {
    
    }

    protected virtual void ChaseTarget() {
        // Turn to face same direction
        if (target.position.x < transform.position.x) {
            controller.FaceLeft();
            direction = -1f;
        } else {
            controller.FaceRight();
            direction = 1f;
        }
        float distance = Mathf.Abs(target.position.x - transform.position.x);
        if (distance < targetDistance) {
            OnTargetReached();
            reached = true;
        }
        // Check that can move
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundAheadCheck.position, groundRadius, whatIsGround);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(wallAheadCheck.position, groundRadius, whatIsGround);
        if (colliders.Length > 0 && colliders2.Length == 0) {
            chasing = true;
        } else {
            StopChasingTarget();
        }
    }

    protected virtual void StopChasingTarget() {
        chasing = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundAheadCheck.position, groundRadius, whatIsGround);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(wallAheadCheck.position, groundRadius, whatIsGround);
        if (colliders.Length == 0 || colliders2.Length > 0) {
            controller.Flip();
            direction *= -1;
        }
    }

    private void FixedUpdate(){
        if (!dead) {

            if (chasing && !reached) {
                controller.Move(direction * chaseSpeed * Time.fixedDeltaTime, false, false);
            } else if (!reached) {
                controller.Move(direction * idleSpeed * Time.fixedDeltaTime, false, false);
            } else {
                controller.Move(0, false, false);
            }
        } else {
            controller.Move(0, false, false);
        }
    }


}
