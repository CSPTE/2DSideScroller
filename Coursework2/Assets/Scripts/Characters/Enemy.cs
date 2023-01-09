using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public Animator animator;
    public BoxCollider2D attackCollider;
    public int damage = 50;

    private bool attacking = false;

    private bool collidingWithPlayer = false;
    private bool attackedPlayer = false;

    public AudioSource growlAudio;
    private bool keepPlayingGrowl = true;
    public AudioSource attackAudio;

    private void Start() {
        target = Player.current.transform;
        health = maxHealth;
        GameObject worldCanvas = GameObject.Find("WorldCanvas");
        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
        healthBar.SetMaxHealth(health);
        attackCollider.enabled = false;
    }

    private void Update() {
        UpdateLogic();
        if (collidingWithPlayer && !attackedPlayer) {
            Player.current.Damage(damage);
            attackedPlayer = true;
        }
        if (attacking) {
            reached = true;
        }
    }

    protected override void OnTargetReached() {
        attacking = true;
        animator.SetBool("Attacking", true);
        attackAudio.Play();
    }

    public void EnableAttackHitBox() {
        attackCollider.enabled = true;
    }

    public void OnAttackFinish() {
        animator.SetBool("Attacking", false);
        attackCollider.enabled = false;
        attackedPlayer = false;
        attacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && !collidingWithPlayer) {
            collidingWithPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collidingWithPlayer) {
            collidingWithPlayer = false;
        }
    }

    protected override void OnDeath() {
        animator.SetBool("Dead", true);
        gameObject.layer = LayerMask.NameToLayer("NPCLayer");
    }

    public void OnDeathComplete() {
        healthBar.Destroy();
        Destroy(healthBar);
        StoryEngine.current.TriggerEvent("EnemyKilled");
    }

}
