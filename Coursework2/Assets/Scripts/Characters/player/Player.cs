using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public static Player current;
    public CharacterController2D controller;
    public Animator animator;
    public BoxCollider2D attackCollider;

    public AudioSource landAudio;
    public AudioSource backgroundMusic;
    public AudioSource attackAudio;

    public float maxHealth = 100;
    public float health = 0;

    public int maxRage = 100;
    public static int rage = 0;

    
    public GameObject healthBarPrefab;
    public GameObject rageMeterPrefab;
    public GameObject timerPrefab;

    public float runSpeed = 40f;

    public float healRate = 10;
    public float timeToHealStart = 5f;

    public int damage = 25;

    private void Awake() {
        current = this;
    }

    public static void ResetValues() {
        rage = 0;
    }

    private float horizontal = 0f;
    private bool jump = false;
    private bool grounded = true;
    private bool attacking = false;
    private bool jumpedThisUpdate = false;
    private bool hideableObjectClose = false;
    private bool hidden = false;

    private bool canDoubleJump = false;
    private bool canAttack = false;
    private bool canWallClimb = false;
    private bool canHide = false;
    private bool died = false;
    private HealthBar healthBar;
    private HealthBar rageMeter;
    private Timer timer;
    

    private float timeTillHealStart = 2f;
    private bool healing = true;
    
    void Start() {
        if (StoryEngine.current.HasOccured("PacifistChoice")) {
            PacifistMode();
        } else if (StoryEngine.current.HasOccured("ViolentChoice")) {
            ViolentMode();
        } else if (SceneManager.GetActiveScene().name == "CastleScene") {
            ViolentMode();
        } else if (SceneManager.GetActiveScene().name == "ForestScene") {
            PacifistMode();
        }
        if (rage == maxRage) {
            StoryEngine.current.TriggerEvent("RageBarFilled");
        }
        backgroundMusic.Play();
        attackCollider.enabled = false;
        health = maxHealth;
        GameObject worldCanvas = GameObject.Find("WorldCanvas");
        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
        healthBar.SetMaxHealth(Mathf.FloorToInt(health));
        StoryEngine.current.EventOccured += EventOccured;
    }
    private void OnDestroy() {
        StoryEngine.current.EventOccured -= EventOccured;
    }

    void EventOccured(String eventType) {
        if (eventType == "EnemyKilled" && rage < maxRage) {
            rage += 10;
            rageMeter.SetHealth(rage);
            if (rage >= maxRage) {
                rage = maxRage;
                StoryEngine.current.TriggerEvent("RageBarFilled");
            }
        }
    }

    public void SetPosition(Transform pos) {
        transform.position = new Vector3(pos.position.x, pos.position.y, transform.position.z);
    }

    public void PacifistMode() {
        StoryEngine.current.TriggerEvent("PacifistChoice");
        if (rageMeter != null) {
            Destroy(rageMeter.gameObject);
            rage = 0;
        }
        transform.localScale = new Vector3(3, 3, 0);

        canDoubleJump = false;
        canAttack = false;
        canWallClimb = true;
        canHide = true;
        controller.changeSettings(canDoubleJump, canWallClimb);
        GameObject canvas = GameObject.Find("Canvas");
        timer = Instantiate(timerPrefab, canvas.transform).GetComponent<Timer>();
        
    }

    public void ViolentMode() {
        StoryEngine.current.TriggerEvent("ViolentChoice");
        if (timer != null) {
            Destroy(timer.gameObject);
        }
        transform.localScale = new Vector3(4, 4, 0);
        canDoubleJump = true;
        canAttack = true;
        canWallClimb = false;
        canHide = false;
        controller.changeSettings(canDoubleJump, canWallClimb);
        GameObject canvas = GameObject.Find("Canvas");
        rageMeter = Instantiate(rageMeterPrefab, canvas.transform).GetComponent<HealthBar>();
        rageMeter.SetMaxHealth(maxRage);
        rageMeter.SetHealth(rage);
    }

    public void Damage(int amount) {
        healing = false;
        timeTillHealStart = timeToHealStart;
        health -= amount;
        if (health < 0) {
            health = 0;
        }
        if (health == 0 && !died) {
            died = true;
            OnDeath();
        }
        healthBar.SetHealth(Mathf.FloorToInt(health));
        animator.SetBool("GettingHit", true);
    }

    private void OnDeath() {
        animator.SetBool("Death", true);
        SceneController.current.PlayerDied();
    }

    private void DeathAnimationComplete() {
        animator.SetBool("Dead", true);
    }

    public void StopGettingHit() {
        animator.SetBool("GettingHit", false);
        if (health == 0) {
            animator.SetBool("Death", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeTillHealStart > 0) {
            timeTillHealStart -= Time.deltaTime;
        }
        
        if (timeTillHealStart <= 0 ) {
            timeTillHealStart = 0;
            healing = true;
        }

        if (healing) {
            health += Time.deltaTime * healRate;
            if (health >= maxHealth) {
                healing = false;
                health = maxHealth;
            }
            healthBar.SetHealth(Mathf.FloorToInt(health));
        }
        if (health == 0) {
            return;
        }
        if (!hidden) {
            if (!attacking || !grounded) {
                horizontal = Input.GetAxisRaw("Horizontal") * runSpeed;
            } else {
                horizontal = 0f;
            }
            if (canAttack && Input.GetButtonDown("Attack")) {
                horizontal = 0f;
                attacking = true;
                animator.SetBool("Attacking", true);
            }
            if (Input.GetButtonDown("Jump")) {
                jump = true;
                jumpedThisUpdate = true;
                animator.SetBool("Jump", true);
            } else if (Input.GetButtonUp("Jump")) {
                jump = false;
            }
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
        } else {
            horizontal = 0f;
            jump = false;
            jumpedThisUpdate = false;
        }

        if (canHide && hideableObjectClose && Input.GetButtonDown("Interact") && !hidden) {
            hidden = true;
            animator.SetBool("Hiding", true);
            gameObject.layer = LayerMask.NameToLayer("NPCLayer");
            StoryEngine.current.TriggerEvent("Hidden");
        } else if (Input.GetButtonDown("Interact") && hidden) {
            hidden = false;
            animator.SetBool("Hiding", false);
            gameObject.layer = LayerMask.NameToLayer("Action");
        }

        healthBar.SetPosition(transform.position);
    }

    private void FixedUpdate() {
        if (health > 0) {
            controller.Move(horizontal * Time.fixedDeltaTime, jump, jumpedThisUpdate);
            jumpedThisUpdate = false;
        } else {
            controller.Move(0, false, false);
        }
    }

    public void OnLanding() {
        //landAudio.Play();
        animator.SetBool("Jump", false);
        jump = false;
        grounded = true;
    }

    public void OnJump() {
        grounded = false;
        jump = true;
        animator.SetBool("Jump", true);
    }

    public void OnClimbStart() {
        animator.SetFloat("Speed", 1f);
    }

    public void OnClimbStop() {
        animator.SetFloat("Speed", 1f);
    }

    public void SetHiding(bool canHide) {
        this.hideableObjectClose = canHide;
    }

    public void OnAttackStart() {
        attackAudio.Play();
        attackCollider.enabled = true;
    }

    public void OnAttackFinish() {
        attacking = false;
        animator.SetBool("Attacking", false);
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy") && attackCollider.enabled) {
            try {
                collision.gameObject.GetComponent<Enemy>().Damage(damage);
            } catch {
                collision.gameObject.GetComponent<RangedEnemy>().Damage(damage);
            }
        }
    }

}
